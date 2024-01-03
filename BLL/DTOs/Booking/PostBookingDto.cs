using Common.Constants;

using FluentValidation;

namespace BLL.DTOs.Booking
{
    public class PostBookingDto
    {
        public bool IsExistingTour { get; set; }
        public int? TourId { get; set; }
        public int PaymentMethod { get; set; }
        public double TotalTime { get; set; }
        public double TotalDistance { get; set; }
        public List<BookingPlaceDto> BookingPlaces { get; set; } = new List<BookingPlaceDto>();
    }

    public class PostBookingDtoValidator : AbstractValidator<PostBookingDto>
    {
        public PostBookingDtoValidator()
        {
            When(x => x.IsExistingTour, () =>
            {
                RuleFor(x => x.TourId).GreaterThan(0).WithMessage("Itinerary Id invalid!");
            });
            RuleFor(x => x.BookingPlaces).Must((places) =>
            {
                bool valid = true;
                places.ForEach((place) =>
                {
                    if (places.Count(x => x.Ordinal == place.Ordinal) > 1)
                    {
                        valid = false;
                        return;
                    }
                });
                return valid;
            }).WithMessage("There are duplicate ordinals in places. Please arrange the places again!");
            RuleFor(x => x.BookingPlaces).Must((places) =>
            {
                bool valid = true;
                places.ForEach((place) =>
                {
                    if (places.Count(x => x.PlaceId == place.PlaceId) > 1)
                    {
                        valid = false;
                        return;
                    }
                });
                return valid;
            }).WithMessage("There are duplicate places in places. Please arrange the places again!");


        }
    }
    public class BookingPlaceDto
    {
        public int PlaceId { get; set; }
        public int Ordinal { get; set; }
        public BookingPlaceStatus Status { get; set; } = BookingPlaceStatus.Future;
    }

}
