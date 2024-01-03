using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AppConfiguration
{
    public class PaypalSettings
    {
        public string BaseAddress { get; set; }
        public string Address { get; set; } 
        public string AuthenticationPath { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public int BookingCancelIntervalInMinutes { get; set; }
    }
}
