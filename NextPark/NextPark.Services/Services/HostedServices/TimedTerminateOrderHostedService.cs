using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NextPark.Services.Services.HostedServices
{
    public class TimedTerminateOrderHostedService : IHostedService, IDisposable
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TimedTerminateOrderHostedService> _logger;
        private Timer _timer;

        public TimedTerminateOrderHostedService(ILogger<TimedTerminateOrderHostedService> logger,
            IHostingEnvironment appEnvironment,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(CheckOutActiveOrders, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void CheckOutActiveOrders(object state)
        {
            var filesFolder = "wwwroot/HostedServiceFiles";  //files folder used by timed hosted services
            DirectoryInfo hostedServicesFilesFolder = new DirectoryInfo(Path.Combine(_appEnvironment.ContentRootPath, filesFolder));
            var files = hostedServicesFilesFolder.GetFiles();

            foreach (var file in files)
            {
                var extension = file.Extension;
                var cutx = file.Name.Split('_');
                var ticks = cutx[0]; //time ticks

                var dateTime = new DateTime(long.Parse(ticks));

                if (DateTime.Now <= dateTime)
                {
                    continue;
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //Getting Repositories
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var userRepository =
                        scope.ServiceProvider.GetRequiredService<IRepository<ApplicationUser>>();
                    var feedRepository = scope.ServiceProvider.GetRequiredService<IRepository<Feed>>();
                    var parkingRepository = scope.ServiceProvider.GetRequiredService<IRepository<Parking>>();
                    var transactionRepository = scope.ServiceProvider.GetRequiredService<IRepository<Transaction>>();

                    var id = int.Parse(cutx[1].Replace(extension, "")); //order id
                    var order = orderRepository.Find(id);

                    if (order == null)
                    {
                        continue;
                    }

                    order.OrderStatus = OrderStatus.Finished;
                    orderRepository.Update(order);
                    
                    _logger.LogInformation($"Order with Id:{id} was finished because end time was reached.{Environment.NewLine}{DateTime.Now}");

                    var parking = parkingRepository.Find(order.ParkingId); //parking related with the order
                    if (parking == null)
                    {
                        _logger.LogInformation("Hosted Service Error: parking associated with tehe order not found");
                        return;
                    }

                    var parkingOwnedUser = userRepository.Find(parking?.UserId); //Parking's owned user
                    var tax = await feedRepository.FirstOrDefaultWhereAsync(f => f.Name == "RentEarningPercent").ConfigureAwait(false);
                    if (tax == null)
                    {
                        _logger.LogInformation("Hosted Service Error: Rent earning tax not found.");
                        return;
                    }
                    var userOrder = userRepository.Find(order.UserId); //User who created the order
                    if (userOrder == null)
                    {
                        _logger.LogInformation("Hosted Service Error: User who created the order not found");
                        return;
                    }
                    var rentTax = CalCulateTax(order.Price, tax.Tax);
                    parking.Status = ParkingStatus.Enabled;

                    var rentTransaction = new Transaction
                    {
                        CashMoved = rentTax,
                        UserId = parking.UserId,
                        CreationDate = DateTime.Now,
                        CompletationDate = DateTime.Now,
                        Status = TransactionStatus.Completed,
                        TransactionId = Guid.NewGuid(),
                        Type = TransactionType.RentTransaction
                    };

                    var feedTransaction = new Transaction
                    {
                        CashMoved = rentTax,
                        UserId = parking.UserId,
                        CreationDate = DateTime.Now,
                        CompletationDate = DateTime.Now,
                        Status = TransactionStatus.Completed,
                        TransactionId = Guid.NewGuid(),
                        Type = TransactionType.FeedTransaction
                    };

                    //Saving rent and feed transactions
                    transactionRepository.Add(rentTransaction);
                    transactionRepository.Add(feedTransaction);

                    //Update user balance when rent is finished
                    userOrder.Balance = userOrder.Balance - order.Price;
                    userRepository.Update(userOrder);

                    //Update user profit when rent is finished
                    parkingOwnedUser.Profit = parkingOwnedUser.Profit + rentTax;
                    userRepository.Update(parkingOwnedUser);

                    //Update parking after status change
                    parkingRepository.Update(parking);

                    //Persist transition
                    await unitOfWork.CommitAsync().ConfigureAwait(false);

                    //Clean file
                    file.Delete();

                }
            }
        }

        private double CalCulateTax(double price, double taxPorcent)
        {
            return (price * taxPorcent) / 100;
        }
    }
}