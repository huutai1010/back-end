using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.Responses;

using DAL.Entities;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AccountResponse<AuthDto>> Login(LoginDto login);
        Task<AccountResponse<AuthDto>> LoginByPhone(LoginByPhoneDto login);
        Task<AccountResponse<AccountListDto>> Register(AccountRegistrationDto accountRegistration);
        Task<AccountResponse<AuthDto>> UpdateProfile(UpdateProfileDto updateProfile);
        Task<AccountResponse<Dictionary<string, string>>> ChangeUserLanguage(int userId, int languageId, string token);
    }
}
