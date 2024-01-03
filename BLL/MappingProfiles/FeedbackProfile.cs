using AutoMapper;
using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.DTOs.Feedback;
using Common.Extensions;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<FeedBack, FeedbacksDto>().ReverseMap();
            CreateMap<FeedBack, FeedbackListDto>()
                .ForMember(m => m.TourId, opt => opt.MapFrom(src => src.ItineraryId))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(src => src.Account.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(src => src.Account.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.Account.Image))
                .ForMember(m => m.NationalImage, opt => opt.MapFrom(src => src.Account.NationalCodeNavigation.Icon));

            CreateMap<AddFeedbackDto, FeedBack>()
                .ForMember(m => m.Rate, opt => opt.MapFrom(src => src.TourRate))
                .ForMember(m => m.Content, opt => opt.MapFrom(src => src.TourContent))
                .ForMember(m => m.ItineraryId, opt => opt.MapFrom(src => src.TourId))
                .AfterMap((dto, entity) =>
                {
                    entity.CreateTime = DateTime.Now;
                    entity.IsPlace = false;
                    entity.Status = 1;

                });

            CreateMap<FeedBack, FeedbackDetailDto>().ReverseMap();

            CreateMap<FeedBack, FeedbackListViewDto>()
                .ForPath(m => m.UserName, opt => opt.MapFrom(src => $" {src.Account.LastName} {src.Account.FirstName}"))
                .ForPath(m => m.National, opt => opt.MapFrom(src => src.Account.NationalCodeNavigation.NationalName))
                .AfterMap((entity, dto) =>
                {
                    if (entity.IsPlace)
                    {
                        dto.Category = "place";
                    }
                    else
                    {
                        dto.Category = "itinerary";
                    }
                });

            CreateMap<AddFeedbackPlaceDto, FeedBack>()
                .ForMember(m => m.Rate, opt => opt.MapFrom(src => src.PlaceRate))
                .ForMember(m => m.Content, opt => opt.MapFrom(src => src.PlaceContent))
               .AfterMap((dto, entity) =>
               {
                   entity.CreateTime = DateTime.Now;
                   entity.IsPlace = true;
                   entity.Status = 1;

               });
        }

    }
}
