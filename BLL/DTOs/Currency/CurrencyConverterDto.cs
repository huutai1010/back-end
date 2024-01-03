using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Currency
{
    public class CurrencyConverterDto
    {
        public long Timestamp { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}
