using AutoMapper;
using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.DTOs.Conversation;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationListDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(m => m.AccountOneFirstName, opt => opt.MapFrom(src => src.AccountOne.FirstName))
                .ForMember(m => m.AccountOneLastName, opt => opt.MapFrom(src => src.AccountOne.LastName))
                .ForMember(m => m.AccountOneImage, opt => opt.MapFrom(src => src.AccountOne.Image))
                .ForMember(m => m.AccountOnePhone, opt => opt.MapFrom(src => src.AccountOne.Phone))
                .ForMember(m => m.AccountTwoFirstName, opt => opt.MapFrom(src => src.AccountTwo.FirstName))
                .ForMember(m => m.AccountTwoLastName, opt => opt.MapFrom(src => src.AccountTwo.LastName))
                .ForMember(m => m.AccountTwoPhone, opt => opt.MapFrom(src => src.AccountTwo.Phone))
                .ForMember(m => m.AccountTwoImage, opt => opt.MapFrom(src => src.AccountTwo.Image))
                .ForMember(m => m.AccountOneUsername, opt => opt.MapFrom(src => src.AccountOne.Email))
                .ForMember(m => m.AccountTwoUsername, opt => opt.MapFrom(src => src.AccountTwo.Email))                
                .ReverseMap();

            CreateMap<AddConversationDto, Conversation>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                });
        }
    }
}