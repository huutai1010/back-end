using BLL.DTOs.Currency;
using BLL.Interfaces;
using Common.AppConfiguration;
using Common.Constants;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BLL.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly INationalityService nationalityService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IWebHostEnvironment _env;
        private readonly ExchangeRatesApiSettings _exchangeRatesSettings;
        private readonly HttpClient _client;

        public CurrencyService(INationalityService nationalityService,
            IOptions<ExchangeRatesApiSettings> settings,
            IRedisCacheService redisCacheService,
            IWebHostEnvironment env)
        {
            _exchangeRatesSettings = settings.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_exchangeRatesSettings.BaseAddress)
            };
            this.nationalityService = nationalityService;
            _redisCacheService = redisCacheService;
            _env = env;
        }
        public async Task LoadExchanges()
        {
            var languages = await nationalityService.GetNationalities();
            List<string> symbolsList = languages.Nationalities.Select(x =>
            {
                switch (x.NationalCode)
                {
                    case "vi": return "VND";
                    case "ja": return "JPY";
                    case "en-us": return "USD";
                    case "sg": return "SGD";
                    case "ru": return "RUB";
                    case "zh-cn": return "CNH";
                    case "en-in": return "INR";
                    case "en-ca": return "CAD";
                    case "ko": return "KRW";
                    default: return "";
                }
            }).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            string symbolsQuery = string.Join(",", symbolsList);
            HttpResponseMessage response;
            CurrencyConverterDto? dto;
            if (!_env.IsDevelopment())
            {
                response = await _client.GetAsync($"latest.json?app_id={_exchangeRatesSettings.ApiKey}&symbols={symbolsQuery}");
                dto = await response.Content.ReadFromJsonAsync<CurrencyConverterDto>();
            }
            else
            {
                dto = new CurrencyConverterDto
                {
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Rates = ExchangeValues.Rates,
                };
            }

            if (dto != null)
            {
                List<CurrencyDto> currencyDtos = new();
                symbolsList.ForEach(symbol =>
                {
                    var currencyDto = new CurrencyDto
                    {
                        Timestamp = dto.Timestamp,
                        Currency = dto.Rates.Keys.First(key => key == symbol),
                        Value = dto.Rates[symbol],
                    };
                    currencyDtos.Add(currencyDto);
                });
                await _redisCacheService.SaveCachePermanentAsync("currency", currencyDtos);
            }
        }
    }
}
