using AutoMapper;
using BLL.DTOs.Category;
using BLL.DTOs.Place;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryLanguage, CategoryViewDto>()
               .ForMember(m => m.Name, opt => opt.MapFrom(src => src.NameLanguage))
               .ForMember(m => m.Id, opt => opt.MapFrom(src => src.CategoryId));

            CreateMap<Category, CategoryListDto>()
                .ForPath(m => m.TotalLanguage, opt => opt.MapFrom(src => src.CategoryLanguages.Count))
                .ReverseMap();
            CreateMap<CategoryDetailDto, Category>().ReverseMap();
            CreateMap<CategoryListCreateDto, Category>().ReverseMap();

            CreateMap<CategoryLanguage, CategoryLanguageDto>()
                .ForPath(m => m.LanguageCode, opt => opt.MapFrom(src => src.LanguageCode.Trim()))
                .ReverseMap();

            CreateMap<CategoryUpdateDto, Category>()
                .AfterMap((dto, entity) =>
                {
                    entity.UpdateTime = DateTime.Now;
                })
                .ReverseMap();

            CreateMap<CategoryCreateDto, Category>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                    entity.CreateTime= DateTime.Now;
                })
                .ReverseMap();

            CreateMap<CategoryLanguageCreateDto, CategoryLanguage>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                })
                .ReverseMap()
;        }
            
    }
}
