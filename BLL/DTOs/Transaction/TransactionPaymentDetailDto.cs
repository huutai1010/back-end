using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class TransactionPaymentDetailDto : BaseDto
    {
        public int TransactionId { get; set; }
        public string PaymentId { get; set; }
        public string CaptureId { get; set; }
        public string PaymentAccountId { get; set; }
        public string Currency { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
