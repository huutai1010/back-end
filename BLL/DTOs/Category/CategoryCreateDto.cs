using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using BLL.DTOs.Place;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Category
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public List<CategoryLanguageCreateDto> CategoryLanguages { get; set; }

        public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
        {
            public CategoryCreateDtoValidator()
            {
                RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
                RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");

                RuleForEach(a => a.CategoryLanguages).SetValidator(new CategoryLanguageCreateDtoValidator());
            }
        }
    }
}
