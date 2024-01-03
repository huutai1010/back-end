using BLL.DTOs.Place.PlaceCategory;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceImage
{
    public class PlaceImageAddDto
    {
        public string? Image { get; set; } 
        public bool IsPrimary { get; set; }
    }

    public class PlaceImageAddDtoValidator : AbstractValidator<PlaceImageAddDto>
    {
        public PlaceImageAddDtoValidator()
        {
            RuleFor(a => a.Image).NotEmpty().WithMessage("Url can't be blank!");

            RuleFor(a => a.IsPrimary).NotNull().WithMessage("IsPrimary can't be blank!");
        }
    }
}
