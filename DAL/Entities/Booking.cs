using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Booking
    {
        public Booking()
        {
            BookingPlaces = new HashSet<BookingPlace>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? ItineraryId { get; set; }
        public decimal Total { get; set; }
        public bool IsPrepared { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<BookingPlace> BookingPlaces { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
