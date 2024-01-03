﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.PlaceDescription
{
    public class PlaceDescriptionDto : BaseDto
    {
        public string? LanguageName { get; set; }
        public string LanguageIcon { get; set; }
        public string LanguageCode { get; set; } = null!;
        public string? VoiceFile { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
    }
}
