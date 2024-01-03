using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceItem
{
    public class PlaceItemViewDto : BaseDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? BeaconId { get; set; }
        public int? BeaconMajorNumber { get; set; }
        public int? BeaconMinorNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int StartTimeInMs { get; set; }
        public int EndTimeInMs { get; set; }
        public string? Image { get; set; }
        public int Status { get; set; }
        public virtual List<ItemDescriptionDto> ItemDescriptions { get; set; }
    }
}
