using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AppConfiguration
{
    public class RabbitmqSettings
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? VHost { get; set; }
        public string? ConsumerQueueName { get; set; }
        public string? ProducerQueueName { get; set;}
    }
}
