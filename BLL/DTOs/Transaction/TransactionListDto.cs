using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class TransactionListDto : BaseDto
    {
        public int BookingId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime CreateTime { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
        public string? PaymentUrl { get; set; }
    }
}
