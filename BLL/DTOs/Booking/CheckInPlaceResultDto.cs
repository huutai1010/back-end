using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Booking
{
    public class CheckInPlaceResultDto
    {
        public int BookingId { get; set; }
        public int BookingPlaceId { get; set; }
        public int PlaceId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
