using AutoMapper;
using BLL.DTOs.Place.PlaceImage;
using DAL.Entities;

namespace BLL.MappingProfiles
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<PlaceImage, PlaceImageDto>()
                  .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Url.FirstOrDefault()))
                  ;
            CreateMap<PlaceImage, PlaceImageListDto>()
                .ForMember(m => m.Image, opt => opt.MapFrom(src => src.Url))
                .ReverseMap();
        }
    }
}
