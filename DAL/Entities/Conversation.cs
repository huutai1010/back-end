using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Conversation
    {
        public int SessionId { get; set; }
        public int AccountOneId { get; set; }
        public int AccountTwoId { get; set; }
        public string ChannelId { get; set; } = null!;
        public int Status { get; set; }

        public virtual Account AccountOne { get; set; } = null!;
        public virtual Account AccountTwo { get; set; } = null!;
    }
}
