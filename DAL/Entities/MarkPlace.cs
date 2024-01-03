using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class MarkPlace
    {
        public int AccountId { get; set; }
        public int PlaceId { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Place Place { get; set; } = null!;
    }
}
