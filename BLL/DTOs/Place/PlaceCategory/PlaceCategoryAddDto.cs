using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceCategory
{
    public class PlaceCategoryAddDto
    {
        public int Id { get; set; }
    }

    public class PlaceCategoryAddDtoValidator : AbstractValidator<PlaceCategoryAddDto>
    {
        public PlaceCategoryAddDtoValidator()
        {
            RuleFor(a => a.Id).NotEmpty().WithMessage("CategoryId can't be blank!");
            RuleFor(a => a.Id).InclusiveBetween(1,int.MaxValue).WithMessage("CategoryId greater than 0 ");
        }
    }
}
