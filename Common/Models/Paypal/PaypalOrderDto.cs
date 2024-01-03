
using Common.AppConfiguration;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Paypal
{
    public class PaypalOrderDto
    { 
        [JsonProperty("intent")]
        public string Intent { get; set; } = "CAPTURE";
        [JsonProperty("purchase_units")]
        public List<PaypalPurchaseUnit> PurchaseUnits { get; set; } = new List<PaypalPurchaseUnit>();
        [JsonProperty("application_context")]
        public PaypalApplicationContext ApplicationContext { get; set; } = null!;

    }
}
