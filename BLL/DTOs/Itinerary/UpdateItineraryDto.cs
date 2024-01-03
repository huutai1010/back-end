using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using BLL.DTOs.Place;
using BLL.DTOs.Tour.TourDescription;
using BLL.DTOs.Tour.TourDetail;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BLL.DTOs.Itinerary.ItineraryDescription;

namespace BLL.DTOs.Tour
{
    public class UpdateItineraryDto
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Total { get; set; }
        public List<ItineraryPlaceAddDto>? TourDetails { get; set; }
        public List<ItineraryDescriptionUpdateDto>? TourDescriptions { get; set; }
    }

    public class UpdateTourDtoValidator : AbstractValidator<UpdateItineraryDto>
    {
        public UpdateTourDtoValidator()
        {

            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.TourDetails).NotEmpty().WithMessage("TourDetails can't be blank!");
            RuleFor(a => a.TourDescriptions).NotEmpty().WithMessage("TourDescriptions can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");

            RuleForEach(a => a.TourDetails).SetValidator(new TourDetailAddDtoValidator());
            RuleForEach(a => a.TourDescriptions).SetValidator(new ItineraryDescriptionUpdateDtoValidator());
        }
    }
}
