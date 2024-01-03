using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Service
{
    public class MessageContent
    {
        public int? senderId { get; set; }
        public string title { get; set; }
        public string content { get; set; }

    }
}
