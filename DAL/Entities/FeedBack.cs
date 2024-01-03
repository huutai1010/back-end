using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class FeedBack
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? PlaceId { get; set; }
        public int? ItineraryId { get; set; }
        public double? Rate { get; set; }
        public string? Content { get; set; }
        public bool IsPlace { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Itinerary? Itinerary { get; set; }
        public virtual Place? Place { get; set; }
    }
}
