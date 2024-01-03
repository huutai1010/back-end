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
    public class DashboardReloaderHostedService : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DashboardReloaderHostedService> _logger;
        private DateTime _nextRun;

        public DashboardReloaderHostedService(IServiceProvider serviceProvider,
            ILogger<DashboardReloaderHostedService> logger)
        {
            _schedule = CrontabSchedule.Parse("0 0 0 * * *",
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
                    var chartService = sp.GetRequiredService<IChartService>();
                    var fromDate = DateTime.Today.AddDays(-1);
                    var toDate = DateTime.Today;
                    await chartService.ReloadDashboard(fromDate, toDate);
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
