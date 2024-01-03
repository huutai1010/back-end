using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Itinerary
{
    public class ItineratyInfoDto : BaseDto
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Total { get; set; }
        public double? Rate { get; set; }
        public int Status { get; set; }
    }
}
