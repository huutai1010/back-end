using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using BLL.DTOs.Place;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BLL.DTOs.Tour.TourDetail
{
    public class TourDetailAddDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public int? Ordinal { get; set; }
    }

    public class TourDetailAddDtoValidator : AbstractValidator<TourDetailAddDto>
    {
        public TourDetailAddDtoValidator()
        {
            RuleFor(a => a.Id).NotEmpty().WithMessage("PlaceId can't be blank!");
            RuleFor(a => a.Id).InclusiveBetween(1, int.MaxValue).WithMessage("PlaceId length is greater than 0!");
        }
    }
}
