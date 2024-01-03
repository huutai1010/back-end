using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Journey
{
    public class JourneyListDto : BaseDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? FirstPlaceName { get; set; }
        public string? LastPlaceName { get; set; }
        public double TotalTime { get; set; }
        public double TotalDistance { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
    }
}
