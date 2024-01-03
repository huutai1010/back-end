using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Journey
{
    public class PostJourneyDto
    {
        public double TotalTime { get; set; }
        public double TotalDistance { get; set; }

        public List<JourneyDetail> JourneyDetails { get; set; }
    }

    public class PostJourneyDtoValidator : AbstractValidator<PostJourneyDto>
    {
        public PostJourneyDtoValidator()
        {            
            RuleFor(x => x.JourneyDetails).Must((places) =>
            {
                bool valid = true;
                places.ForEach((place) =>
                {
                    if (places.Count(x => x.Ordinal == place.Ordinal) > 1)
                    {
                        valid = false;
                        return;
                    }
                });
                return valid;
            }).WithMessage("There are duplicate ordinals in places. Please arrange the places again!");
            RuleFor(x => x.JourneyDetails).Must((places) =>
            {
                bool valid = true;
                places.ForEach((place) =>
                {
                    if (places.Count(x => x.PlaceId == place.PlaceId) > 1)
                    {
                        valid = false;
                        return;
                    }
                });
                return valid;
            }).WithMessage("There are duplicate places in places. Please arrange the places again!");


        }
    }
    public class JourneyDetail
    {
        public int BookingDetailId { get; set; }
        public int PlaceId { get; set; }
        public int Ordinal { get; set; }
    }
}
