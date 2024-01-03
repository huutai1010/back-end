using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class SearchPlaceDto : BaseDto
    {
        public string Name { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public double Rate { get; set; }
        public TimeSpan? Hour { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public string DaysOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? ReviewsCount { get; set; } = 0;

    }
}