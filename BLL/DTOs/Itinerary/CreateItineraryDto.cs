using BLL.DTOs.Tour.TourDescription;
using BLL.DTOs.Tour.TourDetail;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour
{
    public class CreateItineraryDto
    {
        [JsonIgnore]
        public int CreateById { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Total { get; set; }
        public List<ItineraryPlaceAddDto>? TourDetails { get; set; }
        public List<ItineraryDescriptionAddDto>? tourDescriptions { get; set; }

        public class CreateTourDtoValidator : AbstractValidator<CreateItineraryDto>
        {
            public CreateTourDtoValidator()
            {
                RuleFor(a => a.TourDetails).NotEmpty().WithMessage("Place list can't be blank!");
                RuleFor(a => a.tourDescriptions).NotEmpty().WithMessage("Itinerary description list can't be blank!");
                RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
                RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");
                RuleFor(a => a.CreateById).NotEmpty().WithMessage("CreateById can't be blank!");
                RuleForEach(a => a.TourDetails).SetValidator(new TourDetailAddDtoValidator());
                RuleForEach(a => a.tourDescriptions).SetValidator(new TourDescriptionAddDtoValidator());
            }
        }
    }
}
