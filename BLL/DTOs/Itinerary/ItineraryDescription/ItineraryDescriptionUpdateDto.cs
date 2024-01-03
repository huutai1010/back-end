using BLL.DTOs.Tour.TourDescription;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Itinerary.ItineraryDescription
{
    public class ItineraryDescriptionUpdateDto
    {
        public string? LanguageCode { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Status { get; set; }
    }
    public class ItineraryDescriptionUpdateDtoValidator : AbstractValidator<ItineraryDescriptionUpdateDto>
    {
        public ItineraryDescriptionUpdateDtoValidator()
        {
            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");
            RuleFor(a => a.LanguageCode).Length(0, 10).WithMessage("LanguageCode length is not greater than 10!");

            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");
        }
    }

}
