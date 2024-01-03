

namespace BLL.DTOs.Booking.BookingDetail
{
    public class BookingPlaceViewDto
    {
        public int BookingPlaceId { get; set; }
        public int PlaceId { get; set; }
        public int JourneyId { get; set; }
        public bool IsJourney { get; set; }
        public string PlaceName { get; set; } = null!;
        public string PlaceImage { get; set; } = null!;
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartTime { get; set; }
        public int Ordinal { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
