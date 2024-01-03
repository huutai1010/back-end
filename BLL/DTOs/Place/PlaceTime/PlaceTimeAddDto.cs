using BLL.DTOs.Place.PlaceDescription;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceTime
{
    public class PlaceTimeAddDto
    {
        public int DaysOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class PlaceTimeAddDtoValidator : AbstractValidator<PlaceTimeAddDto>
    {
        public PlaceTimeAddDtoValidator()
        {
            RuleFor(a => a.DaysOfWeek).NotEmpty().WithMessage("DaysOfWeek can't be blank!");
            RuleFor(a => a.DaysOfWeek).InclusiveBetween(1, 7).WithMessage("DaysOfWeek range 1 - 7");

           // RuleFor(a => a.OpenTime).NotEmpty().WithMessage("OpenTime can't be blank!");

           // RuleFor(a => a.EndTime).NotEmpty().WithMessage("EndTime can't be blank!");
        }
    }
}
