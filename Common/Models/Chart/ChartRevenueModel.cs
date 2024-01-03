using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Chart
{
    public class ChartRevenueModel
    {
        public string? Date { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalBooking { get; set; }
    }
}
