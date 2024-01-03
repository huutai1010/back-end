using BLL.DTOs.Place.PlaceTime;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceTimeDto
    {
        public int DaysOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int ExcelPlaceId { get; set; }
    }

    public class ExcelPlaceTimeDtoValidator : AbstractValidator<ExcelPlaceTimeDto>
    {
        public ExcelPlaceTimeDtoValidator()
        {
            RuleFor(a => a.DaysOfWeek).NotEmpty().WithMessage("DaysOfWeek can't be blank!");
            RuleFor(a => a.DaysOfWeek).InclusiveBetween(1, 7).WithMessage("DaysOfWeek range 1 - 7");

            RuleFor(a => a.OpenTime).NotEmpty().WithMessage("OpenTime can't be blank!");

            RuleFor(a => a.EndTime).NotEmpty().WithMessage("EndTime can't be blank!");
        }
    }
}
