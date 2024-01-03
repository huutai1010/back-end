using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace JobScheduler
{
    public class JobSchedulers
    {
        private readonly HttpClient _client;

        public JobSchedulers(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("ApiClient");
        }

        [FunctionName("LoadExchanges")]
        public async Task LoadExchanges([TimerTrigger("%LoadExchangesTimer%")]TimerInfo myTimer, ILogger log)
        {
            var response = await _client.GetAsync("api/portal/exchanges/load");
            response.EnsureSuccessStatusCode();
        }

        [FunctionName("LoadDashboard")]
        public async Task LoadDashboard([TimerTrigger("%LoadDashboardTimer%")] TimerInfo myTimer, ILogger log)
        {
            var response = await _client.GetAsync("api/portal/dashboard/load");
            response.EnsureSuccessStatusCode();
        }

        [FunctionName("CancelBooking")]
        public async Task CancelBooking([TimerTrigger("%CancelBookingTimer%")] TimerInfo myTimer, ILogger log)
        {
            var response = await _client.GetAsync("api/bookings/cancel/batch");
            response.EnsureSuccessStatusCode();
        }
    }
}
