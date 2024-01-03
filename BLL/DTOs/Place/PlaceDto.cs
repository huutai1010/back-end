using BLL.DTOs.Language;
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
    public class PlaceDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public decimal Duration { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? Category { get; set; }
        public List<LanguageListDto>? LanguageList { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
    }
}
