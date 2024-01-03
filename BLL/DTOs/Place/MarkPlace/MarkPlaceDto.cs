using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.MarkPlace
{
    public class MarkPlaceDto
    {
        public int PlaceId { get; set; }
        public string Url { get; set; } = null!;
        public int Status { get; set; }
    }
}
