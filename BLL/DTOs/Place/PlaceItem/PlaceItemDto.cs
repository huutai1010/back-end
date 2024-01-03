using DAL.Entities;
using FirebaseAdmin.Auth.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceItem
{
    public class PlaceItemDto : BaseDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string BeaconId { get; set; } = null!;
        public int? BeaconMajorNumber { get; set; }
        public int? BeaconMinorNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int StartTimeInMs { get; set; }
        public int EndTimeInMs { get; set; }
        public string? Url { get; set; }
        public int Status { get; set; }
    }
}
