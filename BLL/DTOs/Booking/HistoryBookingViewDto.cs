
using BLL.DTOs.Journey;
using BLL.DTOs.Place;

namespace BLL.DTOs.Booking
{
    public class HistoryBookingViewDto : BaseDto
    {
        public int? TourId { get; set; }
        public bool IsPrepared { get; set; }
        public decimal Total { get; set; }
        public int TotalPlaces { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public List<PlaceImageViewDto> PlaceImages { get; set; }
    }
}
