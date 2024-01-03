using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class ConfigLanguage
    {
        public ConfigLanguage()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public string FileLink { get; set; } = null!;
        public string LanguageCode { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
