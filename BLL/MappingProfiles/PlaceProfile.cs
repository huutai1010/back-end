using AutoMapper;
using BLL.DTOs.Language;
using BLL.DTOs.Place;
using BLL.DTOs.Place.Excel;
using BLL.DTOs.Place.MarkPlace;
using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceItem;
using BLL.DTOs.Place.PlaceTime;
using Common.Constants;
using DAL.Entities;

using Microsoft.AspNetCore.Routing.Constraints;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            #region PlaceImage
            CreateMap<PlaceImage, PlaceImageDto>().ReverseMap();
            CreateMap<PlaceImageAddDto, PlaceImage>()
                .ForMember(opt => opt.Url, src => src.MapFrom(p => p.Image))
                .AfterMap((dto, entity) =>
            {
                entity.Status = 1;
            }).ReverseMap();
            CreateMap<ExcelPlaceImageDto, PlaceImage>()
                .ForMember(opt => opt.Url, src => src.MapFrom(p => p.Image))
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                }).ReverseMap();
            #endregion

            #region PlaceDescription
            CreateMap<PlaceDescription, PlaceDescriptionDto>()
                .ForMember(m => m.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode.Trim()))
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(PlaceDescStatus), src.Status).ToString()))
                .ReverseMap();
            CreateMap<PlaceDescription,LanguageListDto>().ReverseMap();
            CreateMap<PlaceDescriptionAddDto, PlaceDescription>().AfterMap((dto, entity) =>
            {
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.Status = 1;
            }).ReverseMap();

            CreateMap<PlaceDescriptionUpdateDto, PlaceDescription>().AfterMap((dto, entity) =>
            {
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
            }).ReverseMap();

            CreateMap<ExcelPlaceDescDto, PlaceDescription>().AfterMap((dto, entity) =>
            {
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.Status = 2;
            }).ReverseMap();
            #endregion

            #region PlaceTime
            CreateMap<PlaceTime, PlaceTimeDto>().ReverseMap();
            CreateMap<PlaceTimeAddDto, PlaceTime>().AfterMap((dto, entity) =>
            {
                entity.Status = 1;
            }).ReverseMap();
            CreateMap<ExcelPlaceTimeDto, PlaceTime>().AfterMap((dto, entity) =>
            {
                entity.Status = 1;
            }).ReverseMap();
            #endregion

            #region Place Category
            CreateMap<PlaceCategory, PlaceCategoryDto>()
                .ForMember(opt => opt.Id, src => src.MapFrom(p => p.CategoryId))
                .ForPath(opt => opt.Name, src => src.MapFrom(p => p.Category.Name))
                .ReverseMap();
            CreateMap<PlaceCategoryAddDto, PlaceCategory>()
                .ForMember(opt => opt.CategoryId, src => src.MapFrom(p => p.Id))
                .AfterMap((dto, entity) =>
            {
                entity.Status = 1;
            }).ReverseMap();
            CreateMap<ExcelPlaceCategoryDto, PlaceCategory>()
                .ForMember(opt => opt.CategoryId, src => src.MapFrom(p => p.CategoryId))
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                }).ReverseMap();
            #endregion

            #region Place
            CreateMap<Place, PlaceDto>()
                .ForMember(m => m.LanguageList, opt => opt.MapFrom(src => src.PlaceDescriptions))
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(PlaceStatus), src.Status).ToString()))
                .AfterMap((entity, dto) =>
                {
                    if (entity.Hour != null)
                    {
                        dto.Duration = Math.Round((decimal)entity.Hour.Value.TotalHours, 2);
                    }
                    if (entity.PlaceCategories.SingleOrDefault(x => x.CategoryId == 8) != null)
                    {
                        dto.Category = "food";
                    }
                    else
                    {
                        dto.Category = "travel";
                    }
                })
                .ReverseMap();
            CreateMap<Place, PlaceDetailDto>()
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(PlaceStatus), src.Status).ToString()))
                .ReverseMap();

            CreateMap<Place, PlaceInfoDto>()
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.PlaceImages.FirstOrDefault(x => x.IsPrimary == true).Url))
                .ReverseMap();

            CreateMap<CreatePlaceDto, Place>().AfterMap((dto, entity) =>
            {
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.Status = 1;
            }).ReverseMap();
            CreateMap<UpdatePlaceDto, Place>().AfterMap((dto, entity) =>
            {
                entity.UpdateTime = DateTime.Now;
            }).ReverseMap();
            CreateMap<ExcelPlaceDto, Place>().AfterMap((dto, entity) =>
            {
                if (entity.PlaceImages.FirstOrDefault() != null)
                {
                    entity.PlaceImages.FirstOrDefault().IsPrimary = true;
                }
                entity.CreateTime = DateTime.Now;
                entity.Status = 1;
            }).ReverseMap();

            CreateMap<Place, PlaceViewDto>()
               .ForMember(m => m.Name, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().Name))
               .ForMember(m => m.Description, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().Description))
               .ForMember(m => m.FeedBacks, opt => opt.MapFrom(src => src.FeedBacks))
               .ForMember(m => m.DaysOfWeek, opt => opt.MapFrom(src => Enum.ToObject(typeof(DayOfWeek), src.PlaceTimes.First().DaysOfWeek - 1)))
               .ForMember(m => m.OpenTime, opt => opt.MapFrom(src => src.PlaceTimes.FirstOrDefault().OpenTime))
               .ForMember(m => m.EndTime, opt => opt.MapFrom(src => src.PlaceTimes.FirstOrDefault().EndTime))
               ;

            CreateMap<Place, PlaceVoiceDto>()
               .ForMember(m => m.Name, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().Name))
               .ForMember(m => m.Description, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().Description))
               .ForMember(m => m.LanguageCode, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().LanguageCode))
               .ForMember(m => m.VoiceFile, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().VoiceFile))
               ;

            #endregion

            #region Mark Place
            CreateMap<MarkPlace, MarkPlaceDto>()
                .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Place.PlaceImages.FirstOrDefault()!.Url))
                .ReverseMap();
            CreateMap<AddMarkPlaceDto, MarkPlace>().AfterMap((dto, entity) =>
            {
                entity.Status = 1;
            });
            #endregion

            CreateMap<Place, TopPlaceDto>()
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.PlaceDescriptions.FirstOrDefault().Name))
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.PlaceImages.FirstOrDefault().Url))
                .ForMember(m => m.ReviewsCount, opt => opt.MapFrom(src => src.FeedBacks.Count()));

            CreateMap<Place, SearchPlaceDto>()
               .ForMember(m => m.Name, opt => opt.MapFrom(src => src.PlaceDescriptions.First().Name))
               .ForMember(m => m.Image, opt => opt.MapFrom(src => src.PlaceImages.First().Url))
               .ForMember(m => m.DaysOfWeek, opt => opt.MapFrom(src => Enum.ToObject(typeof(DayOfWeek), src.PlaceTimes.First().DaysOfWeek - 1)))
               .ForMember(m => m.OpenTime, opt => opt.MapFrom(src => src.PlaceTimes.FirstOrDefault().OpenTime))
               .ForMember(m => m.EndTime, opt => opt.MapFrom(src => src.PlaceTimes.FirstOrDefault().EndTime))
               .ForMember(m => m.ReviewsCount, opt => opt.MapFrom(src => src.FeedBacks.Count()));

            #region PlaceItem
            CreateMap<PlaceItem, PlaceItemDto>()
               .ForMember(m => m.Name, opt => opt.MapFrom(src => src.ItemDescriptions.FirstOrDefault().NameItem))
               .ForMember(m => m.StartTimeInMs, opt => opt.MapFrom(src => src.StartTime.TotalMilliseconds))
               .ForMember(m => m.EndTimeInMs, opt => opt.MapFrom(src => src.EndTime.TotalMilliseconds));            
            CreateMap<ItemDescription, ItemDescriptionDto>()
                .ForMember(m => m.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode.Trim()))
                .ReverseMap();
            CreateMap<PlaceItem, PlaceItemViewDto>()
                .ForMember(m => m.StartTimeInMs, opt => opt.MapFrom(src => src.StartTime.TotalMilliseconds))
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.Url))
                .ForMember(m => m.EndTimeInMs, opt => opt.MapFrom(src => src.EndTime.TotalMilliseconds));
            CreateMap<PlaceAddItemDto, PlaceItem>()
                .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Image))
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                })
                .ReverseMap();

            CreateMap<PlaceUpdateItemDto, PlaceItem>()
                .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Image))
                .ReverseMap();  

            CreateMap<ExcelPlaceItemDto, PlaceItem>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                })
                .ReverseMap();
            CreateMap<ItemDescriptionAddDto, ItemDescription>()
                .ReverseMap();
            CreateMap<ItemDescriptionUpdateDto, ItemDescription>()
                .ReverseMap();
            CreateMap<ExcelItemDescDto, ItemDescription>()
                .ReverseMap();
            #endregion
        }
    }
}
