using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceItem;
using BLL.DTOs.Place.PlaceTime;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class UpdatePlaceDto
    {
        public string Name { get; set; } = null!;
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Address { get; set; } = null!;
        public string? GooglePlaceId { get; set; }
        public decimal? EntryTicket { get; set; }
        public TimeSpan Hour { get; set; }
        public decimal? Price { get; set; }
        public List<PlaceCategoryAddDto>? PlaceCategories { get; set; }
        public List<PlaceImageAddDto>? PlaceImages { get; set; }
        public List<PlaceDescriptionUpdateDto>? PlaceDescriptions { get; set; }
        public List<PlaceTimeAddDto>? PlaceTimes { get; set; }
        public List<PlaceUpdateItemDto>? PlaceItems { get; set; }
    }

    public class UpdatePlaceDtoValidator : AbstractValidator<UpdatePlaceDto>
    {
        public UpdatePlaceDtoValidator()
        {
            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");

            RuleFor(a => a.Longitude).NotEmpty().WithMessage("Longitude can't be blank!");
            RuleFor(a => a.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.");

            RuleFor(a => a.Latitude).NotEmpty().WithMessage("Latitude can't be blank!");
            RuleFor(a => a.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.");

            RuleFor(a => a.Hour).NotEmpty().WithMessage("Hour can't be blank!");
            RuleFor(a => a.Address).NotEmpty().WithMessage("Address can't be blank!");

            RuleFor(a => a.Price).NotEmpty().WithMessage("Price can't be blank!");

            RuleForEach(a => a.PlaceCategories).SetValidator(new PlaceCategoryAddDtoValidator());
            RuleForEach(a => a.PlaceImages).SetValidator(new PlaceImageAddDtoValidator());
            RuleForEach(a => a.PlaceDescriptions).SetValidator(new PlaceDescriptionUpdateDtoValidator());
            RuleForEach(a => a.PlaceTimes).SetValidator(new PlaceTimeAddDtoValidator());
            RuleForEach(a => a.PlaceItems).SetValidator(new PlaceUpdateItemDtoValidator());
        }
    }
}
