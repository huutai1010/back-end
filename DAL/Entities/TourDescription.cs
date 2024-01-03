using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class TourDescription
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string? LanguageCode { get; set; }

        public virtual Tour Tour { get; set; } = null!;
    }
}
