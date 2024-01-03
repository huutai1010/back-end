using BLL.DTOs.Place.PlaceImage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceImageDto
    {
        public string? Image { get; set; }
        public bool IsPrimary { get; set; }
        public int ExcelPlaceId { get; set; }
    }

    public class ExcelPlaceImageDtoValidator : AbstractValidator<ExcelPlaceImageDto>
    {
        public ExcelPlaceImageDtoValidator()
        {
            RuleFor(a => a.Image).NotEmpty().WithMessage("Url can't be blank!");

            RuleFor(a => a.IsPrimary).NotNull().WithMessage("IsPrimary can't be blank!");
        }
    }
}
