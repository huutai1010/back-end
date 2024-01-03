using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class BookingPlace
    {
        public BookingPlace()
        {
            CelebrateImages = new HashSet<CelebrateImage>();
        }

        public int Id { get; set; }
        public int PlaceId { get; set; }
        public int BookingId { get; set; }
        public int? JourneyId { get; set; }
        public decimal Price { get; set; }
        public DateTime? StartTime { get; set; }
        public int? Ordinal { get; set; }
        public int Status { get; set; }
        public bool IsJourney { get; set; }
        public DateTime? ExpiredTime { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Journey? Journey { get; set; }
        public virtual Place Place { get; set; } = null!;
        public virtual ICollection<CelebrateImage> CelebrateImages { get; set; }
    }
}
