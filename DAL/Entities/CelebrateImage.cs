using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class CelebrateImage
    {
        public int Id { get; set; }
        public int BookingDetailId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int Status { get; set; }

        public virtual BookingPlace BookingDetail { get; set; } = null!;
    }
}
