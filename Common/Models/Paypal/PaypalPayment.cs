using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Paypal
{
    public class PaypalPayment
    {
        [JsonProperty("captures")]
        public List<PaypalCapture> Captures { get; set; }
    }
}
