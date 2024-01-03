using AutoMapper;
using BLL.DTOs.Itinerary;
using BLL.DTOs.Itinerary.ItineraryDescription;
using BLL.DTOs.Place;
using BLL.DTOs.Tour;
using BLL.DTOs.Tour.TourDescription;
using BLL.DTOs.Tour.TourDetail;
using Common.Constants;
using DAL.Entities;

namespace BLL.MappingProfiles
{
    public class TourProfile : Profile
    {
        public TourProfile()
        {
            #region Tour
            CreateMap<Itinerary, ItineraryListOperation>()
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(TourStatus), src.Status).ToString()))
                .ReverseMap();
            CreateMap<Itinerary, ItineraryPlaceOpDto>()
                .ForMember(m => m.tourDetails, opt => opt.MapFrom(src => src.TourDetails.OrderBy(x => x.Ordinal)))
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(TourStatus), src.Status).ToString()))
                .ReverseMap();
            CreateMap<UpdateItineraryDto, Itinerary>()
                .AfterMap((dto, entity) =>
                {
                    entity.CreateTime = DateTime.Now;
                    entity.UpdateTime = DateTime.Now;
                    entity.Status = 1;
                })
                .BeforeMap((dto, entity) =>
                {
                    int ordinal = 1;
                    foreach (var item in dto.TourDetails)
                    {
                        item.Ordinal = ordinal;
                        ordinal++;
                    }
                })
                .ReverseMap();

            CreateMap<CreateItineraryDto, Itinerary>()
                .BeforeMap((dto, entity) =>
                {
                    int ordinal = 1;
                    foreach (var item in dto.TourDetails)
                    {
                        item.Ordinal = ordinal;
                        ordinal++;
                    }
                })
                .AfterMap((dto, entity) =>
                {
                    entity.CreateTime = DateTime.Now;
                    entity.UpdateTime = DateTime.Now;
                    entity.Status = 1;

            }).ReverseMap();

            CreateMap<ItineratyInfoDto, Itinerary>().ReverseMap();

            CreateMap<Itinerary, ItineraryViewDto>()
                .ForMember(m => m.Price, opt => opt.MapFrom(src => src.Total))
                 .ForMember(m => m.Name, opt => opt.MapFrom(src => src.TourDescriptions.First().Name))
                 .ForMember(m => m.Description, opt => opt.MapFrom(src => src.TourDescriptions.First().Description))
                 .ForMember(m => m.Places, opt => opt.MapFrom(src => src.TourDetails));


            CreateMap<ItineraryPlace, TopPlaceDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Place.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Place.PlaceDescriptions.First().Name))
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.Place.PlaceImages.First().Url));
            CreateMap<ItineraryPlace, SearchPlaceDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Place.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Place.PlaceDescriptions.First().Name))
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.Place.PlaceImages.First().Url))
                .ForMember(m => m.Longitude, opt => opt.MapFrom(src => src.Place.Longitude))
                .ForMember(m => m.Latitude, opt => opt.MapFrom(src => src.Place.Latitude))
                .ForMember(m => m.GooglePlaceId, opt => opt.MapFrom(src => src.Place.GooglePlaceId))
                .ForMember(m => m.Hour, opt => opt.MapFrom(src => src.Place.Hour))
                .ForMember(m => m.Price, opt => opt.MapFrom(src => src.Place.Price))
                .ForMember(m => m.Rate, opt => opt.MapFrom(src => src.Place.Rate))
               .ForMember(m => m.DaysOfWeek, opt => opt.MapFrom(src => Enum.ToObject(typeof(DayOfWeekEnum), src.Place.PlaceTimes.First().DaysOfWeek - 1)))
               .ForMember(m => m.OpenTime, opt => opt.MapFrom(src => src.Place.PlaceTimes.FirstOrDefault().OpenTime))
               .ForMember(m => m.EndTime, opt => opt.MapFrom(src => src.Place.PlaceTimes.FirstOrDefault().EndTime));
            #endregion

            #region Tour Detail
            CreateMap<ItineraryPlace, ItineraryPlaceDto>()
                .ForMember(opt => opt.Id, src => src.MapFrom(p => p.Place.Id))
                .ForPath(opt => opt.Name, src => src.MapFrom(p => p.Place.Name))
                .ForPath(opt => opt.Rate, src => src.MapFrom(p => p.Place.Rate))
                .ForPath(opt => opt.Duration, src => src.MapFrom(p => p.Place.Hour))
                .ReverseMap();
            CreateMap<ItineraryPlaceAddDto, ItineraryPlace>()
                .ForMember(opt => opt.PlaceId, src => src.MapFrom(p => p.Id))
                .ReverseMap();
            #endregion

            #region Tour Description
            CreateMap<ItineraryDescription, ItineraryDescriptionDto>()
                 .ForMember(m => m.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode.Trim()))
                 .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(TourDescStatus), src.Status).ToString()))
                .ReverseMap();
            CreateMap<ItineraryDescriptionAddDto, ItineraryDescription>().AfterMap((dto, entity) =>
            {
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.Status = 1;
            }).ReverseMap();

            CreateMap<ItineraryDescriptionUpdateDto, ItineraryDescription>().AfterMap((dto, entity) =>
            {
                entity.UpdateTime = DateTime.Now;
            }).ReverseMap();

            CreateMap<Itinerary, TopTourDto>()
                .ForMember(m => m.Price, opt => opt.MapFrom(src => src.Total))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.TourDescriptions.First().Name))
                .ForMember(m => m.ReviewsCount, opt => opt.MapFrom(src => src.FeedBacks.Count()));
            #endregion

        }
    }
}
