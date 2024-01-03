using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class TransactionBookingOverviewDto
    {
        public int? TourId { get; set; }
        public bool IsPrepared { get; set; }
        public decimal Total { get; set; }
        public int TotalPlaces { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
