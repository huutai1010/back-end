using BLL.DTOs.Feedback;
using BLL.DTOs.Tour.TourDescription;
using BLL.DTOs.Tour.TourDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour
{
    public class ItineraryPlaceOpDto : BaseDto
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Total { get; set; }
        public double? Rate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
        public List<ItineraryDescriptionDto>? tourDescriptions { get; set; }
        public List<ItineraryPlaceDto>? tourDetails { get; set; }
        public List<FeedbacksDto>? FeedBacks { get; set; }
    }
}
