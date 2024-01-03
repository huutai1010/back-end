using BLL.DTOs.Place.PlaceCategory;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Category
{
    public class CategoryLanguageCreateDto
    {
        public string NameLanguage { get; set; } = null!;
        public string LanguageCode { get; set; } = null!;
    }

    public class CategoryLanguageCreateDtoValidator : AbstractValidator<CategoryLanguageCreateDto>
    {
        public CategoryLanguageCreateDtoValidator()
        {
            RuleFor(a => a.NameLanguage).NotEmpty().WithMessage("NameLanguage can't be blank!");
            RuleFor(a => a.NameLanguage).MaximumLength(150).WithMessage("NameLanguage length is not greater than 150!");

            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");
            RuleFor(a => a.LanguageCode).MaximumLength(10).WithMessage("LanguageCode length is not greater than 10!");
        }
    }
}
