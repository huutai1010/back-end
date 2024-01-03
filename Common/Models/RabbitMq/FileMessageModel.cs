using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RabbitMq
{
    public class FileMessageModel
    {
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public int status { get; set; }
    }
}
