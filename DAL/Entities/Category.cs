using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Category
    {
        public Category()
        {
            CategoryLanguages = new HashSet<CategoryLanguage>();
            PlaceCategories = new HashSet<PlaceCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual ICollection<CategoryLanguage> CategoryLanguages { get; set; }
        public virtual ICollection<PlaceCategory> PlaceCategories { get; set; }
    }
}
