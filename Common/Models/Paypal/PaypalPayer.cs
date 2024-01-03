using Newtonsoft.Json;

namespace Common.Models.Paypal
{
    public class PaypalPayer
    {
        [JsonProperty("name")]
        public PaypalPayerName Name { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }
        [JsonProperty("address")]
        public PayPalAddress Address { get; set; }
    }
}