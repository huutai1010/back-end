using BLL.DTOs.Feedback;
using BLL.DTOs.Place;
using BLL.DTOs.Place.PlaceImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour
{
    public class ItineraryViewDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public Dictionary<string, double> Exchanges { get; set; }
        public double? Rate { get; set; }

        public List<SearchPlaceDto> Places { get; set; }
        public List<FeedbackListDto> FeedBacks { get; set; }

        public List<PlaceImageDto> PlaceImages { get; set; }

    }
}
