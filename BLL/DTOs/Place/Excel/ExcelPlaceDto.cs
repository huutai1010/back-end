using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceDto
    {
        public int ExcelPlaceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Address { get; set; } = null!;
        public string? GooglePlaceId { get; set; }
        public decimal? EntryTicket { get; set; }
        public TimeSpan Hour { get; set; }
        public decimal? Price { get; set; }
        public List<ExcelPlaceCategoryDto>? PlaceCategories { get; set; }
        public List<ExcelPlaceImageDto>? PlaceImages { get; set; }
        public List<ExcelPlaceDescDto>? PlaceDescriptions { get; set; }
        public List<ExcelPlaceTimeDto>? PlaceTimes { get; set; }
        public List<ExcelPlaceItemDto>? PlaceItems { get; set; }

        public class ExcelPlaceDtoValidator : AbstractValidator<ExcelPlaceDto>
        {
            public ExcelPlaceDtoValidator()
            {
                RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
                RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");

                RuleFor(a => a.Longitude).NotEmpty().WithMessage("Longitude can't be blank!");

                RuleFor(a => a.Latitude).NotEmpty().WithMessage("Latitude can't be blank!");

                RuleFor(a => a.Hour).NotEmpty().WithMessage("Hour can't be blank!");
                RuleFor(a => a.Address).NotEmpty().WithMessage("Address can't be blank!");

                RuleFor(a => a.Price).NotEmpty().WithMessage("Price can't be blank!");

                RuleForEach(a => a.PlaceCategories).SetValidator(new ExcelPlaceCategoryDtoValidator());
                RuleForEach(a => a.PlaceImages).SetValidator(new ExcelPlaceImageDtoValidator());
                RuleForEach(a => a.PlaceDescriptions).SetValidator(new ExcelPlaceDescDtoValidator());
                RuleForEach(a => a.PlaceTimes).SetValidator(new ExcelPlaceTimeDtoValidator());
                RuleForEach(a => a.PlaceItems).SetValidator(new ExcelPlaceItemDtoValidator());
            }
        }
    }
}
