

namespace BLL.DTOs.Booking.BookingDetail
{
    public class JourneyPlaceViewDto
    {
        public int BookingPlaceId { get; set; }
        public int PlaceId { get; set; }
        public int JourneyId { get; set; }
        public string PlaceName { get; set; } = null!;
        public string DefaultName { get; set; } = null!;
        public string PlaceImage { get; set; } = null!;
        public string DaysOfweek { get; set; } = null!;
        public TimeSpan OpenTime { get; set; } 
        public TimeSpan CloseTime { get; set; } 
        public decimal Longitude { get; set; }
        public TimeSpan? Hour { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int Ordinal { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public bool IsSupport { get; set; }
    }
}
