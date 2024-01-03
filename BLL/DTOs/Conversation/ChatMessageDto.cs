using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Conversation
{
    public class ChatMessageDto
    {
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public string Content { get; set; }
    }
}
