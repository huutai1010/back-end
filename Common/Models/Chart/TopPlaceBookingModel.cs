using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Chart
{
    public class TopPlaceBookingModel
    {
        public int PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public int TotalBooking { get; set; }
    }
}
