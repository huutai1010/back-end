using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class TransactionDetail
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string PaymentId { get; set; } = null!;
        public string? CaptureId { get; set; }
        public string? PaymentAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }

        public virtual Transaction Transaction { get; set; } = null!;
    }
}
