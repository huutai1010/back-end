using BLL.DTOs.Currency;
using BLL.Interfaces;
using Common.AppConfiguration;
using Common.Constants;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using System.Net.Http.Json;

namespace BLL.Services
{
    public class CurrencyHostedService : BackgroundService
    {
        private readonly ExchangeRatesApiSettings _exchangeRatesSettings;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyHostedService> _logger;


        public CurrencyHostedService(IServiceProvider serviceProvider, IOptions<ExchangeRatesApiSettings> settings, ILogger<CurrencyHostedService> logger)
        {
            _exchangeRatesSettings = settings.Value;
            _schedule = CrontabSchedule.Parse(_exchangeRatesSettings.Schedule,
                new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = DateTime.Now;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_exchangeRatesSettings.BaseAddress)
            };
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            do
            {
                try
                {
                    var now = DateTime.Now;
                    if (now >= _nextRun)
                    {
                        bool succeeded = await Process();
                        if (succeeded)
                        {
                            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                            _logger.LogInformation($"Next run at: {_nextRun}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    // Do nothing
                }
                finally
                {
                    await Task.Delay(5000, stoppingToken);
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
                    var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                    var currencyService =
                    scope.ServiceProvider
                        .GetRequiredService<ICurrencyService>();

                    await currencyService.LoadExchanges();

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
