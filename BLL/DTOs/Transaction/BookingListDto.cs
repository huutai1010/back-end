using Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class BookingListDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Total { get; set; }
        public int TotalPlace { get; set; }
        public decimal TotalTime { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
    }
}
