using AutoMapper;
using BLL.DTOs.Language;
using DAL.Entities;

namespace BLL.MappingProfiles
{
    public class LanguageProfile : Profile
    {
        public LanguageProfile()
        {
            CreateMap<ConfigLanguage, LanguageDto>().ReverseMap();
            CreateMap<AddLanguageDto, ConfigLanguage>().ReverseMap();

            CreateMap<ConfigLanguage, LanguageViewOpDto>()
                .AfterMap((entity, dto) =>
                {
                    dto.Quantity = entity.Accounts.Count();
                });
        }
    }
}
