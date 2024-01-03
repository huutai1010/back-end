using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour
{
    public class ItineraryListOperation : BaseDto
    {
        public string? Name { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Total { get; set; }
        public int TotalPlace { get; set; }
        public int Status { get; set; }
        public double? Rate { get; set; }
        public string? StatusType { get; set; }

    }
}
