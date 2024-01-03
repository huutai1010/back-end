using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class CategoryLanguage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string NameLanguage { get; set; } = null!;
        public int? Status { get; set; }
        public string LanguageCode { get; set; } = null!;

        [JsonIgnore]
        public virtual Category Category { get; set; } = null!;
    }
}
