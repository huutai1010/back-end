using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class ItemDescription
    {
        public int Id { get; set; }
        public int PlaceItemId { get; set; }
        public string LanguageCode { get; set; } = null!;
        public string NameItem { get; set; } = null!;

        public virtual PlaceItem PlaceItem { get; set; } = null!;
    }
}
