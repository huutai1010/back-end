using BLL.DTOs.Place.PlaceCategory;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceDescription
{
    public class PlaceDescriptionAddDto
    {
        public string? LanguageCode { get; set; }
        public string? VoiceFile { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class PlaceDescriptionAddDtoValidator : AbstractValidator<PlaceDescriptionAddDto>
    {
        public PlaceDescriptionAddDtoValidator()
        {
            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");

            RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
            RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length not greater than 200!");

            RuleFor(a => a.Description).NotEmpty().WithMessage("Description can't be blank!");
        }
    }
}
