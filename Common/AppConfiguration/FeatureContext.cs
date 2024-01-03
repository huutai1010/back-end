using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AppConfiguration
{
    public class FeatureContext
    {
        public bool ExchangeRates { get; set; }
        public bool CancelBooking { get; set; }
        public bool ReloadDashboard { get; set; }
    }
}
