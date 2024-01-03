using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceTime
{
    public class PlaceTimeDto
    {
        public int Id { get; set; }
        public int DaysOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
