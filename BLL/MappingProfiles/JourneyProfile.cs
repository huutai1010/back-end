using AutoMapper;
using BLL.DTOs.Journey;
using BLL.DTOs.Place.CelebratedPlace;
using Common.Constants;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class JourneyProfile : Profile
    {
        public JourneyProfile()
        {
            CreateMap<Journey, JourneyListDto>()
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => 
                (JourneyStatus)Enum.ToObject(typeof(JourneyStatus), src.Status)))
                 .ForMember(m => m.FirstPlaceName, opt => opt.MapFrom(src => src.BookingPlaces.OrderBy(x => x.Ordinal).First().Place.PlaceDescriptions.FirstOrDefault().Name))
                .ForMember(m => m.LastPlaceName, opt => opt.MapFrom(src => src.BookingPlaces.OrderBy(x => x.Ordinal).Last().Place.PlaceDescriptions.FirstOrDefault().Name));

            CreateMap<PostJourneyDto, Journey>()
                .AfterMap((_, journey) =>
                {
                    journey.Status = (int)JourneyStatus.Future;
                    journey.StartTime = DateTime.Now;
                });

            CreateMap<Journey, JourneyViewDto>()
                .ForMember(m => m.BookingPlaces, opt => opt.MapFrom(src => src.BookingPlaces))
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src =>
                (JourneyStatus)Enum.ToObject(typeof(JourneyStatus), src.Status)));
        }
    }
}
