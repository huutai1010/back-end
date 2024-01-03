using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Currency
{
    public class CurrencyDto
    {
        public long Timestamp { get; set; }
        public string Currency { get; set; }
        public double Value { get; set; }
    }
}
