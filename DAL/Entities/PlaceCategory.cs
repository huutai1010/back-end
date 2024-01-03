using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class PlaceCategory
    {
        public int PlaceId { get; set; }
        public int CategoryId { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Place Place { get; set; } = null!;
    }
}
