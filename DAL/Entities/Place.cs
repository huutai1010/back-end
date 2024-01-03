using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Place
    {
        public Place()
        {
            BookingPlaces = new HashSet<BookingPlace>();
            FeedBacks = new HashSet<FeedBack>();
            TourDetails = new HashSet<ItineraryPlace>();
            MarkPlaces = new HashSet<MarkPlace>();
            PlaceCategories = new HashSet<PlaceCategory>();
            PlaceDescriptions = new HashSet<PlaceDescription>();
            PlaceImages = new HashSet<PlaceImage>();
            PlaceItems = new HashSet<PlaceItem>();
            PlaceTimes = new HashSet<PlaceTime>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public string Address { get; set; } = null!;
        public decimal? EntryTicket { get; set; }
        public TimeSpan? Hour { get; set; }
        public double? Rate { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual ICollection<BookingPlace> BookingPlaces { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<ItineraryPlace> TourDetails { get; set; }
        public virtual ICollection<MarkPlace> MarkPlaces { get; set; }
        public virtual ICollection<PlaceCategory> PlaceCategories { get; set; }
        public virtual ICollection<PlaceDescription> PlaceDescriptions { get; set; }
        public virtual ICollection<PlaceImage> PlaceImages { get; set; }
        public virtual ICollection<PlaceItem> PlaceItems { get; set; }
        public virtual ICollection<PlaceTime> PlaceTimes { get; set; }
    }
}
