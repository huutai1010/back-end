using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class PlaceImage
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public int Status { get; set; }

        public virtual Place Place { get; set; } = null!;
    }
}
