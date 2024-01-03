using AutoMapper;
using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.DTOs.Feedback;
using BLL.DTOs.Staff;
using BLL.DTOs.Visitor;
using Common.Constants;
using Common.Extensions;

using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AuthDto>()
                .ForMember(m => m.LanguageId, opt => opt.MapFrom(src => src.ConfigLanguage.Id))
                .ForMember(m => m.LanguageCode, opt => opt.MapFrom(src => src.ConfigLanguage.LanguageCode))
                .ForMember(m => m.Nationality, opt => opt.MapFrom(src => src.NationalCodeNavigation.NationalName))
                .ForMember(m => m.NationalCode, opt => opt.MapFrom(src => src.NationalCodeNavigation.NationalCode))
                .ForMember(m => m.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<Account, AccountListDto>()
                .ForMember(m => m.Role, opt => opt.MapFrom(src => src.RoleId));

            CreateMap<AccountRegistrationDto, Account>()
                .ForMember(m => m.Password, opt => opt.MapFrom(src => PasswordExtension.HashPassword(src.Password)))
                .AfterMap((dto, entity) =>
                {
                    entity.CreateTime = DateTime.Now;
                    entity.RoleId = 3;
                    entity.Status = 1;

                });

            CreateMap<ChangePasswordDto, Account>()
                .ForMember(m => m.Password, opt => opt.MapFrom(src => PasswordExtension.HashPassword(src.NewPassword)))
                .ReverseMap();

            CreateMap<UpdateProfileDto, Account>().AfterMap((dto, entity) =>
            {
                entity.UpdateTime = DateTime.Now;
            });

            CreateMap<AccountUpdateDto, Account>()
                .AfterMap((dto, entity) =>
                {
                    entity.UpdateTime = DateTime.Now;
                })
                .ReverseMap();

            CreateMap<StaffUpdateDto, Account>()
                .AfterMap((dto, entity) =>
                {
                    entity.UpdateTime = DateTime.Now;
                })
                .ReverseMap();

            CreateMap<AccountDto, Account>() .ReverseMap();

            CreateMap<Account, AccountInforDto>()
                .ForMember(dto => dto.CustomerName, opt => opt.MapFrom(src => src.LastName + " " + src.FirstName))
                .ForPath(dto => dto.Nationality, opt => opt.MapFrom(src => src.NationalCodeNavigation.NationalName))
                .ReverseMap();

            #region Visitor
            CreateMap<Account, VisitorListDto>() 
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(src => src.LastName + " " + src.FirstName))
                .ForPath(dto => dto.Nationality, opt => opt.MapFrom(src => src.NationalCodeNavigation.NationalName))
                .ReverseMap();

            CreateMap<Account, VisitorDetailDto>()
                .ForPath(dto => dto.Nationality, opt => opt.MapFrom(src => src.NationalCodeNavigation.NationalName))
                .ForMember(dto => dto.TransactionsHistory, opt => opt.MapFrom(src => src.Bookings.SelectMany(b => b.Transactions)))
                .ReverseMap();

            CreateMap<Transaction, TransactionHistoryDto>()
                .AfterMap((entity, dto) =>
                {
                    dto.StatusType = Enum.ToObject(typeof(TransactionStatus), entity.Status).ToString();
                })
                .ReverseMap();
            #endregion

            #region Staff
            CreateMap<Account, StaffListDto>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(src => src.LastName + " " + src.FirstName))
                .AfterMap((entity, dto) =>
                {
                    if (entity.RoleId == 2)
                    {
                        dto.Role = "Moderator";
                    }
                    else
                    {
                        dto.Role = "Administrator";
                    }
                })
                .ReverseMap();

            CreateMap<CreateStaffDto, Account>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                    entity.ConfigLanguageId = 1;
                    entity.CreateTime = DateTime.Now;
                    entity.Password = PasswordExtension.HashPassword(dto.Password);
                })
                .ReverseMap();
            CreateMap<Account, StaffDetailDto>().ReverseMap();
            CreateMap<Account, FeedbackAccountDto>().ReverseMap();
            #endregion
        }
    }
}
