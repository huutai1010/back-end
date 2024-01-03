using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionDetails = new HashSet<TransactionDetail>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Booking Booking { get; set; } = null!;
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
