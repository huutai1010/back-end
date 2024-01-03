using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class ItineraryDescription
    {
        public int Id { get; set; }
        public int ItineraryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string? LanguageCode { get; set; }

        public virtual Itinerary Tour { get; set; } = null!;
    }
}
