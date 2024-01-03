using BLL.DTOs.Place.PlaceItem;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelItemDescDto
    {
        public int ExcelItemId { get; set; }
        public string? LanguageCode { get; set; }
        public string? NameItem { get; set; }
    }

    public class ExcelItemDescDtoValidator : AbstractValidator<ExcelItemDescDto>
    {
        public ExcelItemDescDtoValidator()
        {
            RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");
            RuleFor(a => a.LanguageCode).MaximumLength(10).WithMessage("LanguageCode length is not greater than 10!");
            RuleFor(a => a.NameItem).NotEmpty().WithMessage("NameItem can't be blank!");
            RuleFor(a => a.NameItem).MaximumLength(50).WithMessage("NameItem length is not greater than 50!");
        }
    }
}
