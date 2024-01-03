using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class ItineraryPlace
    {
        public int PlaceId { get; set; }
        public int ItineraryId { get; set; }
        public decimal Price { get; set; }
        public int? Ordinal { get; set; }

        public virtual Place Place { get; set; } = null!;
        public virtual Itinerary Tour { get; set; } = null!;
    }
}
