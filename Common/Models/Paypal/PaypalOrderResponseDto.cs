using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Paypal
{
    public class PaypalOrderResponseDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("purchase_units")]
        public List<PaypalPurchaseUnit> PurchaseUnits { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("links")]
        public List<Link> Links { get; set; }
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
    }
}
