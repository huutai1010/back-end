using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Conversation
{
    public class VideoCallDto
    {
        public string ChannelName { get; set; }
        public string Token { get; set; }
        public int ReceivingUserId { get; set; }
    }
}
