using BLL.DTOs.Feedback;
using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceItem;
using BLL.DTOs.Place.PlaceTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class PlaceDetailDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public decimal? Price { get; set; }
        public decimal? EntryTicket { get; set; }
        public double? Rate { get; set; }
        public string? Address { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public TimeSpan? Hour { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
        public virtual ICollection<PlaceTimeDto>? PlaceTimes { get; set; }
        public virtual ICollection<PlaceCategoryDto>? PlaceCategories { get; set; }
        public virtual ICollection<PlaceImageListDto>? PlaceImages { get; set; }
        public virtual ICollection<PlaceDescriptionDto>? PlaceDescriptions { get; set; }
        public virtual ICollection<PlaceItemViewDto>? PlaceItems { get; set; }
    }
}
