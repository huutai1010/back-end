using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class HistoryTransactionDto : BaseDto
    {
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
    }
}
