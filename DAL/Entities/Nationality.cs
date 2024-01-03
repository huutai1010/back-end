using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Nationality
    {
        public Nationality()
        {
            Accounts = new HashSet<Account>();
        }

        public string PhoneCode { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string NationalName { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public string LanguageName { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
