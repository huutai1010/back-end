using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceDescription
{
    public class PlaceDescriptionUpdateDto
    {
        public string? LanguageCode { get; set; }
        public string? VoiceFile { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
    }

    public class PlaceDescriptionUpdateDtoValidator : AbstractValidator<PlaceDescriptionUpdateDto> 
    { 
        public PlaceDescriptionUpdateDtoValidator()
        {
            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");

            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length not greater than 200!");

            RuleFor(a => a.Description).NotEmpty().WithMessage("Description can't be blank!");
        }
    }
}
