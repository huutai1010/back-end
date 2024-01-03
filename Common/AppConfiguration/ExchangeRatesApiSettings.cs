using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AppConfiguration
{
    public class ExchangeRatesApiSettings
    {
        public string BaseAddress { get; set; }
        public string ApiKey { get; set; }
        public int IntervalMinutes { get; set; }
        public string Schedule { get; set; }
    }
}
