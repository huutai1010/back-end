using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace BLL.DTOs.Feedback
{
    public class AddFeedbackDto
    {
        public int JourneyId { get; set; }
        public int? TourId { get; set; }
        public double? TourRate { get; set; }
        public string? TourContent { get; set; }

        public List<AddFeedbackPlaceDto>? Places { get; set; }
    }

    public class AddFeedbackPlaceDto
    {
        public int? PlaceId { get; set; }
        public double? PlaceRate { get; set; }
        public string? PlaceContent { get; set; }

    }

}
