using AutoMapper;
using BLL.DTOs.Booking.BookingDetail;
using Common.Constants;
using DAL.Entities;
using DayOfWeek = System.DayOfWeek;

namespace BLL.MappingProfiles
{
    public class BookingPlaceProfile : Profile
    {
        public BookingPlaceProfile() {
            CreateMap<BookingPlace, BookingPlaceViewDto>()
                .ForMember(m => m.PlaceName, opt => opt.MapFrom(src => src.Place.PlaceDescriptions.FirstOrDefault().Name))
                .ForMember(m => m.PlaceImage, opt => opt.MapFrom(src => src.Place.PlaceImages.FirstOrDefault().Url))
                .ForMember(m => m.GooglePlaceId, opt => opt.MapFrom(src => src.Place.GooglePlaceId))
                .ForMember(m => m.Latitude, opt => opt.MapFrom(src => src.Place.Latitude))
                .ForMember(m => m.BookingPlaceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Longitude, opt => opt.MapFrom(src => src.Place.Longitude))
                .ForMember(m => m.JourneyId, opt => opt.MapFrom(src => src.JourneyId))
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => Enum.ToObject(typeof(BookingPlaceStatus), src.Status)));
        CreateMap<BookingPlace, JourneyPlaceViewDto>()
               .ForMember(m => m.PlaceName, opt => opt.MapFrom(src => src.Place.PlaceDescriptions.FirstOrDefault().Name))
               .ForMember(m => m.DefaultName, opt => opt.MapFrom(src => src.Place.Name))
                .ForMember(m => m.PlaceImage, opt => opt.MapFrom(src => src.Place.PlaceImages.FirstOrDefault().Url))
               .ForMember(m => m.DaysOfweek, opt => opt.MapFrom(src => Enum.ToObject(typeof(DayOfWeek), src.Place.PlaceTimes.First().DaysOfWeek - 1)))
               .ForMember(m => m.OpenTime, opt => opt.MapFrom(src => src.Place.PlaceTimes.First().OpenTime))
               .ForMember(m => m.CloseTime, opt => opt.MapFrom(src => src.Place.PlaceTimes.First().EndTime))
                .ForMember(m => m.GooglePlaceId, opt => opt.MapFrom(src => src.Place.GooglePlaceId))
                .ForMember(m => m.Latitude, opt => opt.MapFrom(src => src.Place.Latitude))
                .ForMember(m => m.BookingPlaceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Longitude, opt => opt.MapFrom(src => src.Place.Longitude))
                .ForMember(m => m.Hour, opt => opt.MapFrom(src => src.Place.Hour))
                .ForMember(m => m.JourneyId, opt => opt.MapFrom(src => src.JourneyId))
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => Enum.ToObject(typeof(BookingPlaceStatus), src.Status)));
        }
    }
}

