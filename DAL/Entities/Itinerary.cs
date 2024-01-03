using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Itinerary
    {
        public Itinerary()
        {
            FeedBacks = new HashSet<FeedBack>();
            TourDescriptions = new HashSet<ItineraryDescription>();
            TourDetails = new HashSet<ItineraryPlace>();
        }

        public int Id { get; set; }
        public int CreateById { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal Total { get; set; }
        public double? Rate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Account CreateBy { get; set; } = null!;
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<ItineraryDescription> TourDescriptions { get; set; }
        public virtual ICollection<ItineraryPlace> TourDetails { get; set; }
    }
}
