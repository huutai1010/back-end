using BLL.DTOs.Paypal;

using Newtonsoft.Json;

namespace Common.Models.Paypal
{
    public class PaypalPurchaseUnit
    {
        [JsonProperty("amount")]
        public PaypalBaseAmount Amount { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("items")]
        public List<PaypalPurchaseUnitItem> Items { get; set; }
        [JsonProperty("payments")]
        public PaypalPayment Payments { get; set; }
    }
}