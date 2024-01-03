using BLL.DTOs.Place.PlaceItem;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceItemDto
    {
        public int ExcelPlaceId { get; set; }
        public int ExcelItemId { get; set; }
        public string? Name { get; set; }
        public string? BeaconId { get; set; }
        public string? Url { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? BeaconMajorNumber { get; set; }
        public int? BeaconMinorNumber { get; set; }
        public List<ExcelItemDescDto>? ItemDescriptions { get; set; }
    }

    public class ExcelPlaceItemDtoValidator : AbstractValidator<ExcelPlaceItemDto>
    {
        public ExcelPlaceItemDtoValidator()
        {
            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(50).WithMessage("Name length is not greater than 50!");

            RuleFor(a => a.BeaconId).NotEmpty().WithMessage("BeaconId can't be blank!");
            RuleFor(a => a.BeaconMajorNumber).NotEmpty().WithMessage("BeaconMajorNumber can't be blank!");
            RuleFor(a => a.BeaconMinorNumber).NotEmpty().WithMessage("BeaconMinorNumber can't be blank!");

            RuleFor(a => a.BeaconId).MaximumLength(255).WithMessage("BeaconId length is not greater than 255!");

            RuleFor(a => a.StartTime).NotEmpty().WithMessage("StartTime can't be blank!");

            RuleFor(a => a.EndTime).NotEmpty().WithMessage("EndTime can't be blank!")
                .GreaterThan(a => a.StartTime).WithMessage("EndTime must be greater than OpenTime"); ;

            RuleForEach(a => a.ItemDescriptions).SetValidator(new ExcelItemDescDtoValidator());

        }
    }
}
