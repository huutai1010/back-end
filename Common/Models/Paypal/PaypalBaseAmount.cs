using Newtonsoft.Json;

namespace Common.Models.Paypal
{
    public class PaypalBaseAmount
    {
        [JsonProperty("currency_code")]

        public string CurrencyCode { get; set; } = "USD";
        [JsonProperty("value")]

        public string Value { get; set; }
        
    }
}