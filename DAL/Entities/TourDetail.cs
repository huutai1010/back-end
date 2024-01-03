using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class TourDetail
    {
        public int PlaceId { get; set; }
        public int TourId { get; set; }
        public decimal Price { get; set; }
        public int? Ordinal { get; set; }

        public virtual Place Place { get; set; } = null!;
        public virtual Tour Tour { get; set; } = null!;
    }
}
