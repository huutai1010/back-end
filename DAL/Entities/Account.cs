using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Account
    {
        public Account()
        {
            Bookings = new HashSet<Booking>();
            ConversationAccountOnes = new HashSet<Conversation>();
            ConversationAccountTwos = new HashSet<Conversation>();
            FcmTokens = new HashSet<FcmToken>();
            FeedBacks = new HashSet<FeedBack>();
            Itineraries = new HashSet<Itinerary>();
            MarkPlaces = new HashSet<MarkPlace>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int ConfigLanguageId { get; set; }
        public int RoleId { get; set; }
        public string? NationalCode { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual ConfigLanguage ConfigLanguage { get; set; } = null!;
        public virtual Nationality? NationalCodeNavigation { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Conversation> ConversationAccountOnes { get; set; }
        public virtual ICollection<Conversation> ConversationAccountTwos { get; set; }
        public virtual ICollection<FcmToken> FcmTokens { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<Itinerary> Itineraries { get; set; }
        public virtual ICollection<MarkPlace> MarkPlaces { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
