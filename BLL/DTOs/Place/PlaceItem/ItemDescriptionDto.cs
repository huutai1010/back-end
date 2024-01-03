using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceItem
{
    public class ItemDescriptionDto : BaseDto
    {
        public int PlaceItemId { get; set; }
        public string LanguageCode { get; set; } = null!;
        public string NameItem { get; set; } = null!;
    }
}
