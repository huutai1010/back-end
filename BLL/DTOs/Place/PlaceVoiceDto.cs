using BLL.DTOs.Feedback;
using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place
{
    public class PlaceVoiceDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string LanguageCode { get; set; } = null!;
        public string? VoiceFile { get; set; }
        public decimal? EntryTicket { get; set; }
        public double? Rate { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? GooglePlaceId { get; set; }
        public string Address { get; set; } = null!;
        public List<PlaceImageDto> PlaceImages { get; set; }
    }
}