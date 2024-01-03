using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class PlaceInfoDto : BaseDto
    {
        public string Name { get; set; } 
        public string Image { get; set; } 
        public double? Rate { get; set; }
        public decimal? Price { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
    }
}
