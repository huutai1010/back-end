using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class PlaceDescription
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string? VoiceFile { get; set; }
        public string Name { get; set; } = null!;
        public string LanguageCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Place Place { get; set; } = null!;
    }
}
