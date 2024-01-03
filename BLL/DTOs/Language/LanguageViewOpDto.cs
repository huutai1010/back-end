using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Language
{
    public class LanguageViewOpDto : BaseDto
    {
        public string? Icon { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? LanguageCode { get; set; }
        public Decimal? Ratio { get; set; }
    }
}
