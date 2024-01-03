using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models.Paypal;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Common.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly PaypalSettings _paypalSettings;
        private HttpClient _client;

        public PaypalService(IOptions<PaypalSettings> options)
        {
            _paypalSettings = options.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri(options.Value.BaseAddress)
            };
            _client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _client.DefaultRequestHeaders
                  .Add("Prefer", "return=representation");
        }

        public async Task<PaypalOrderCaptureDto?> CapturePayment(string orderId)
        {
            await SetAuthToken();
            var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"v2/checkout/orders/{orderId}/capture", httpContent);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaypalOrderCaptureDto>(responseData);
        }

        public async Task<PaypalOrderResponseDto?> CreatePayment(PaypalOrderDto order)
        {
            await SetAuthToken();
            order.ApplicationContext = new PaypalApplicationContext(_paypalSettings);
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("v2/checkout/orders", content);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaypalOrderResponseDto>(responseData);
        }

        private async Task SetAuthToken()
        {
            var existingToken = _client.DefaultRequestHeaders.Authorization;
            if (existingToken == null)
            {
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                };
                var content = new FormUrlEncodedContent(values);

                var authenticationString = $"{_paypalSettings.ClientId}:{_paypalSettings.ClientSecret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _paypalSettings.AuthenticationPath);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                requestMessage.Content = content;

                var response = await _client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Dictionary<string, dynamic> responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody) ?? throw new Exception();

                string newToken = responseData["access_token"];
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {newToken}");
            }
        }
    }
}
