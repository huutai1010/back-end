using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Category
{
    public class CategoryUpdateDto
    {
        public string Name { get; set; }
        public List<CategoryLanguageCreateDto> CategoryLanguages { get; set; }

        public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
        {
            public CategoryUpdateDtoValidator()
            {
                RuleFor(a => a.Name).NotEmpty().WithMessage("Name can't be blank!");
                RuleFor(a => a.Name).MaximumLength(200).WithMessage("Name length is not greater than 200!");

                RuleForEach(a => a.CategoryLanguages).SetValidator(new CategoryLanguageCreateDtoValidator());
            }
        }
    }
}
