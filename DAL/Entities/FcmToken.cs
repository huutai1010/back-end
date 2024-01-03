using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class FcmToken
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Token { get; set; } = null!;
        public bool IsPrimary { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
