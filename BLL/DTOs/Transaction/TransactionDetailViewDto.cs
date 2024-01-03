using BLL.DTOs.Booking;
using BLL.DTOs.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class TransactionDetailViewDto :BaseDto
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public TransactionBookingOverviewDto Booking { get; set; } = null!;
        public TransactionPaymentDetailDto Details { get; set; }
    }
}
