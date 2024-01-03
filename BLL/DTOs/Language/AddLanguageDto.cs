using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace BLL.DTOs.Language
{
    public class AddLanguageDto
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string FileLink { get; set; }
        public string LanguageCode { get; set; }
    }

    public class AddLanguageDtoValidator : AbstractValidator<AddLanguageDto>
    {
        public AddLanguageDtoValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("Language name can't be blank!");
            RuleFor(m => m.Icon).NotEmpty().WithMessage("Language icon can't be blank!");
            RuleFor(m => m.FileLink).NotEmpty().WithMessage("Language file can't be blank!");
            RuleFor(m => m.LanguageCode).NotEmpty().WithMessage("Language code can't be blank!");
        }
    }
}
