
using FluentValidation;
using System.Text.Json.Serialization;

namespace BLL.DTOs.Tour.TourDetail
{
    public class ItineraryPlaceAddDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public int? Ordinal { get; set; }
    }

    public class TourDetailAddDtoValidator : AbstractValidator<ItineraryPlaceAddDto>
    {
        public TourDetailAddDtoValidator()
        {
            RuleFor(a => a.Id).NotEmpty().WithMessage("PlaceId can't be blank!");
            RuleFor(a => a.Id).InclusiveBetween(1, int.MaxValue).WithMessage("PlaceId length is greater than 0!");
        }
    }
}
