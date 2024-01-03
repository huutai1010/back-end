using BLL.DTOs.Place.CelebratedPlace;

namespace BLL.DTOs.Booking.BookingDetail
{
    public class CelebratedPlaceDto 
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public DateTime? StartTime { get; set; }
        public int Ordinal { get; set; }

         public List<CelebratedImageDto>? CelebrateImages { get; set; }
    }
}
