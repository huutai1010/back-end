using BLL.DTOs.Place.PlaceCategory;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceCategoryDto
    {
        public int CategoryId { get; set; }
        public int PlaceExcelId { get; set; }
    }

    public class ExcelPlaceCategoryDtoValidator : AbstractValidator<ExcelPlaceCategoryDto>
    {
        public ExcelPlaceCategoryDtoValidator()
        {
            RuleFor(a => a.CategoryId).NotEmpty().WithMessage("CategoryId can't be blank!");
            RuleFor(a => a.CategoryId).InclusiveBetween(1, int.MaxValue).WithMessage("CategoryId greater than 0 ");
        }
    }
}
