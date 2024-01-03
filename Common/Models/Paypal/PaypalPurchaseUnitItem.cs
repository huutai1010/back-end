using Newtonsoft.Json;

namespace BLL.DTOs.Paypal
{
    public class PaypalPurchaseUnitItem
    {
        [JsonProperty("name")]

        public string Name { get; set; }
        [JsonProperty("quantity")]

        public string Quantity { get; set; }
        [JsonProperty("description")]

        public string Description { get; set; }
    }
}