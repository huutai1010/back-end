using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class PlaceItem
    {
        public PlaceItem()
        {
            ItemDescriptions = new HashSet<ItemDescription>();
        }

        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string BeaconId { get; set; } = null!;
        public int? BeaconMajorNumber { get; set; }
        public int? BeaconMinorNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Url { get; set; }
        public int Status { get; set; }

        public virtual Place Place { get; set; } = null!;
        public virtual ICollection<ItemDescription> ItemDescriptions { get; set; }
    }
}
