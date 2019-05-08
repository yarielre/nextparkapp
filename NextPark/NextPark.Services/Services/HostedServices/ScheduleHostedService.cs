using Microsoft.Extensions.Configuration;
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
    public class ScheduleHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ScheduleHostedService> _logger;
        private Timer _timer;
        private  int _scheduleTimer;


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
            _timer = new Timer(ScheduleTerminateOrderTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(_scheduleTimer));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public async void ScheduleTerminateOrderTask(object state)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scheduleRepository = scope.ServiceProvider.GetRequiredService<IRepository<Schedule>>();
                var orderSchedules = await scheduleRepository.FindAllWhereAsync(sch => sch.ScheduleType == ScheduleType.Order);
                foreach (var schedule in orderSchedules)
                {
                    if  (DateTime.Now < schedule.TimeOfExecution)
                    {
                        continue;
                    }
                    //Service to handle terminate order logic
                    var orderApiService = scope.ServiceProvider.GetRequiredService<IOrderApiService>();
                    var terminateOrderApiResponse = await orderApiService.TerminateOrder(schedule.ScheduleId);
                    if (terminateOrderApiResponse.IsSuccess)
                    {
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

        private async void CheckOutActiveOrders(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                //Service in charge to create, get and delete order's file 
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                var files = fileService.GetHostedFiles();
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
                        //Service to handle terminate order logic
                        var orderApiService = scope.ServiceProvider.GetRequiredService<IOrderApiService>();

                        var id = int.Parse(cutx[1].Replace(extension, "")); //order id
                        var terminateOrderApiResponse = await orderApiService.TerminateOrder(id);
                        if (terminateOrderApiResponse.IsSuccess)
                        {
                            _logger.LogInformation($"Hosted Service Msg {DateTime.Now}: Order with Id:{id} was finished because end time was reached.");
                            //Clean file
                            file.Delete();
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
    }
