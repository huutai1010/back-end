using Newtonsoft.Json;

namespace Common.Models.Paypal
{
    public class PayPalAddress
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}