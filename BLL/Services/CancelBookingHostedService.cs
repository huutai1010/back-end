using BLL.DTOs;
using BLL.Interfaces;
using Common.Constants;
using Common.Interfaces;
using Google.Api.Gax;
using Google.Apis.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace BLL.Services
{
    public class CancelBookingHostedService : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CancelBookingHostedService> _logger;
        private DateTime _nextRun;

        public CancelBookingHostedService(IServiceProvider serviceProvider,
            ILogger<CancelBookingHostedService> logger)
        {
            _schedule = CrontabSchedule.Parse("*/15 * * * * *",
                new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = DateTime.Now;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                bool error = false;
                try
                {
                    var now = DateTime.Now;
                    if (now >= _nextRun)
                    {
                        error = await Process();
                        if (error)
                        {
                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            _logger.LogInformation($"Next run at: {_nextRun}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    error = true;
                    _logger.LogError(ex.Message);
                }
                finally
                {   
                    if (error)
                    {
                        // Restart background service if error
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task<bool> Process()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var sp = scope.ServiceProvider;
                    var redisCacheService = sp.GetRequiredService<IRedisCacheService>();
                    var allJobs = await redisCacheService.Get<List<CancelBookingSchedulerJobDto>>(RedisCacheKeys.CANCEL_BOOKING);
                    if (allJobs == null || !allJobs.Any())
                    {
                        return true;
                    }
                    var now = DateTime.Now;
                    var bookingService = sp.GetRequiredService<IBookingService>();
                    var jobsRemain = new List<CancelBookingSchedulerJobDto>();
                    foreach (var job in allJobs)
                    {
                        if (now > job.ExpiredTime)
                        {
                            await bookingService.CancelBooking(job.BookingId);
                        }
                        else
                        {
                            jobsRemain.Add(job);
                        }
                    }
                    await redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.CANCEL_BOOKING, jobsRemain);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
        }
    }
}
