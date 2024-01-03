using BLL.DTOs.Booking.BookingDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Journey
{
    public class JourneyViewDto : BaseDto
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double TotalTime { get; set; }
        public double TotalDistance { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
        public int? TourId { get; set; }
        public List<JourneyPlaceViewDto> BookingPlaces { get; set; }
    }
}

