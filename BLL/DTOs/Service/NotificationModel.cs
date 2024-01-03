using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Service
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FcmToken { get; set; }
        public bool Status { get; set; }
    }
}
