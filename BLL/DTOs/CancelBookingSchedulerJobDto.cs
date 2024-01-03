using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class CancelBookingSchedulerJobDto
    {
        public int BookingId { get; set; }
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public DateTime ExpiredTime { get; set; }
        public double JourneyTotalTime { get; set; }
        public double JourneyTotalDistance { get; set; }
        public bool IsJourney { get; set; }
    }
}
