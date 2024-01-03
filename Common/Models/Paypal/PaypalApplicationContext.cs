using Common.AppConfiguration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Common.Models.Paypal
{
    public class PaypalApplicationContext
    {
        public PaypalApplicationContext(PaypalSettings settings)
        {
            ReturnUrl = settings.ReturnUrl;
            CancelUrl = settings.CancelUrl;
        }
        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
    }
}