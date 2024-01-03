using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceImage
{
    public class PlaceImageListDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Image { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
