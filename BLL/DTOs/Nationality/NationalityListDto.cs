using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Nationality
{
    public class NationalityListDto
    {
        public string? PhoneCode { get; set; }
        public string? NationalCode { get; set; }
        public string? NationalName { get; set; }
        public string? Icon { get; set; }
        public string? LanguageName { get; set; }
        public int Status { get; set; }
    }
}
