using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NextPark.Services.Services.HostedServices
{
    public class ScheduleHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ScheduleHostedService> _logger;
        private Timer _timer;
        private int _scheduleTimer;


        public ScheduleHostedService(ILogger<ScheduleHostedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _scheduleTimer = int.Parse(configuration["ScheduleTimer"]);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(SchedulerConsumerTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(_scheduleTimer));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public async void SchedulerConsumerTask(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scheduleRepository = scope.ServiceProvider.GetRequiredService<IRepository<Schedule>>();
                var unitOfWotk = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                try
                {
                    var notifySchedules = await scheduleRepository.FindAllWhereAsync(sch => sch.ScheduleType == ScheduleType.Notify);

                    var notificationsSent = await ProcessNotify(scope, notifySchedules);

                    if (notificationsSent.Count > 0)
                    {
                        foreach (var schedule in notificationsSent)
                        {
                            scheduleRepository.Delete(schedule);
                        }
                        await unitOfWotk.CommitAsync().ConfigureAwait(false);
                    }

                }
                catch (Exception e)
                {

                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Notify schedule exception:{e.Message}");

                }

                try
                {
                    var orderSchedules = await scheduleRepository.FindAllWhereAsync(sch => sch.ScheduleType == ScheduleType.Order);
                    await ProcessOrders(scope, orderSchedules);
                }
                catch (Exception e)
                {

                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Order Terminate schedule exception:{e.Message}");

                }


            }
        }
        private async Task<List<Schedule>> ProcessNotify(IServiceScope scope, System.Collections.Generic.List<Schedule> notifySchedules)
        {
            var notificationSent = new List<Schedule>();

            foreach (var schedule in notifySchedules)
            {
                if (DateTime.Now <= schedule.TimeOfExecution)
                {
                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Notify schedule:{schedule.ScheduleId} not yet fired. Will be fired at: {schedule.TimeOfExecution}");
                    continue;
                }


                var usersRepository = scope.ServiceProvider.GetRequiredService<IRepository<ApplicationUser>>();
                var deviceRepository = scope.ServiceProvider.GetRequiredService<IRepository<Device>>();
                var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();

                var currentUser = await usersRepository.SingleOrDefaultWhereAsync(user => user.Id == schedule.ScheduleId);

                if (currentUser == null)
                {
                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Used with Id:{schedule.ScheduleId} not found");
                    continue;
                }

                var userDevices = await deviceRepository.FindAllWhereAsync(device => device.UserId == currentUser.Id);
                if (userDevices == null || userDevices.Count == 0)
                {
                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Used with Id:{schedule.ScheduleId} has any registered device!");
                    continue;
                }

                currentUser.Devices = userDevices;

                //TODO: Send notifications Here
                pushService.Notify(currentUser, $"Scheduled  Order closure", "Order closed", $"Your last order will been closed automatically in 10 min!");

                notificationSent.Add(schedule);
            }

            return notificationSent;
        }
        private async Task ProcessOrders(IServiceScope scope, System.Collections.Generic.List<Schedule> orderSchedules)
        {
            foreach (var schedule in orderSchedules)
            {
                if (DateTime.Now <= schedule.TimeOfExecution)
                {
                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Order terminate schedule:{schedule.ScheduleId} not yet fired. Will be fired at: {schedule.TimeOfExecution}");
                    continue;
                }
                //Service to handle terminate order logic
                var orderApiService = scope.ServiceProvider.GetRequiredService<IOrderApiService>();
                
                var terminateOrderApiResponse = await orderApiService.TerminateOrder(schedule.ScheduleId);
                if (terminateOrderApiResponse.IsSuccess)
                {
                    //TODO: Send notifications Here
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();
                    var usersRepository = scope.ServiceProvider.GetRequiredService<IRepository<ApplicationUser>>();
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IRepository<Device>>();
                    var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();

                    var currentOrder = await orderRepository.SingleOrDefaultWhereAsync(order => order.Id == schedule.ScheduleId);

                    var currentUser = await usersRepository.SingleOrDefaultWhereAsync(user => user.Id == currentOrder.UserId);

                    var userDevices = await deviceRepository.FindAllWhereAsync(device => device.UserId == currentUser.Id);
                    if (userDevices != null &&  userDevices.Count > 0)
                    {

                        currentUser.Devices = userDevices;
                        pushService.Notify(currentUser, $"Scheduled  Order closure", "Order closed", $"Your order {currentOrder.Id} has been closed automatically!");

                    }
                    
                    _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Order with Id:{schedule.ScheduleId} was finished because end time was reached.");
                }
                else
                {
                    _logger.LogInformation($"Hosted Service Error: {DateTime.Now}: {terminateOrderApiResponse.Message} {Environment.NewLine}" +
                                           $" Error Type:{Enum.GetName(typeof(ErrorType), terminateOrderApiResponse.ErrorType)} ");
                }
            }
        }
    }
}
