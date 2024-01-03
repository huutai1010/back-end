using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AppConfiguration
{
    public class RedisSettings
    {
        public string BaseAddress { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public double AbsoluteExpirationInMinutes { get; set; } = 10;
        public double SlidingExpirationInMinutes { get; set; } = 2;
    }
}
