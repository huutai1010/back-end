using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour.TourDetail
{
    public class ItineraryPlaceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int? Ordinal { get; set; }
        public TimeSpan? Duration { get; set; }
        public double? Rate { get; set; }
    }
}
