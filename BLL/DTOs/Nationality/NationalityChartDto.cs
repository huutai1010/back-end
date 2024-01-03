using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Nationality
{
    public class NationalityChartDto
    {
        public string NationalName { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public int? Quantity { get; set; }
        public Decimal? Ratio { get; set; }
    }
}
