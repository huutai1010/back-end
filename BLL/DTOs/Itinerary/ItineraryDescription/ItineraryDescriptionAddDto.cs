using BLL.DTOs.Tour.TourDetail;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Tour.TourDescription
{
    public class ItineraryDescriptionAddDto
    {
        public string? LanguageCode { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class TourDescriptionAddDtoValidator : AbstractValidator<ItineraryDescriptionAddDto>
    {
        public TourDescriptionAddDtoValidator()
        {
            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");
            RuleFor(a => a.LanguageCode).Length(0,10).WithMessage("LanguageCode length is not greater than 10!");

            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");
        }
    }
}
