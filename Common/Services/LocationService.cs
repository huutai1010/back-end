using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class LocationService : ILocationService
    {
        private HttpClient _client;

        public LocationService(IOptions<LocationApiSettings> locationApiSettingsOptions)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(locationApiSettingsOptions.Value.BaseAddress)
            };
        }
        public async Task<List<UserLocationData>> GetUserLocationDatasAsync(int userId, string languageCode)
        {
            var response = await _client.GetAsync($"online?languageCode={languageCode}");
            var responseData = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<UserLocationData>>(responseData);

            return result ?? throw new Exception("Data not found.");
        }
    }
}
