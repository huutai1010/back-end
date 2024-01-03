using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class TopPlaceDto : BaseDto
    {
        public string Name { get; set; }
        public double Rate { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int ReviewsCount { get; set; }

    }
}