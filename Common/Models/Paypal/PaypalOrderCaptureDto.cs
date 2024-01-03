using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Paypal
{
    public class PaypalOrderCaptureDto
    {
        [JsonProperty("id")]
        public string OrderId { get; set; }
        [JsonProperty("intent")]
        public string Intent { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("purchase_units")]
        public List<PaypalPurchaseUnit> PurchaseUnits { get; set; }
        [JsonProperty("payer")]
        public PaypalPayer Payer { get; set; }
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

    }
}
