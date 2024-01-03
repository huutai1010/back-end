using AutoMapper;
using BLL.DTOs.Language;
using BLL.DTOs.Nationality;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class NationalityProfile : Profile
    {
        public NationalityProfile()
        {
            CreateMap<NationalityListDto, Nationality>().ReverseMap();

            CreateMap<Nationality, NationalityChartDto>()
                .AfterMap((entity, dto) =>
                {
                    dto.Quantity = entity.Accounts.Count();
                });
        }
    }
}
