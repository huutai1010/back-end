using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;

        public AuthController(IAuthService authService, IAccountService accountService)
        {
            _authService = authService; _accountService = accountService;
        }

        [HttpPost("admin/login")]
        [SwaggerOperation(Summary = "[Admin] Login", Description = "Authenticate to get token to access the API for ADMIN", OperationId = "loginAdmin")]
        public async Task<AccountResponse<AuthDto>> Login([FromBody] LoginDto req)
        {
            return await _authService.Login(req);
        }
        [HttpPost("user/login")]
        [SwaggerOperation(Summary = "[Visitor] Login", Description = "Authenticate to get token to access the API for Visitor", OperationId = "loginUser")]
        public async Task<AccountResponse<AuthDto>> LoginByPhone([FromBody] LoginByPhoneDto req)
        {
            return await _authService.LoginByPhone(req);
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "[Visitor] Register", Description = "Register to access the API.", OperationId = "register")]
        public async Task<AccountResponse<AccountListDto>> Register([FromBody] AccountRegistrationDto accountRegistration)
        {
            return await _authService.Register(accountRegistration);
        }

        [HttpPut("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Update Profile", Description = "Update current user profile", OperationId = "updateProfile")]
        public async Task<AccountResponse<AuthDto>> UpdateProfile([FromBody] UpdateProfileDto updateProfile)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userIdToUpdate);
            if (!isValid || userIdToUpdate != updateProfile.Id)
            {
                throw new ForbiddenException();
            }
            var result = await _authService.UpdateProfile(updateProfile);
            return result;
        }

        [HttpPut("languages/{languageId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Change Language", Description = "Change current user's language. After changing the language, new token will be generated with new language attached.", OperationId = "changeLanguage")]
        public async Task<IActionResult> ChangeLanguage([FromRoute] int languageId)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token == null)
            {
                return Unauthorized();
            }
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userIdToUpdate);
            if (!isValid || userIdToUpdate <= 0)
            {
                throw new ForbiddenException();
            }

            var result = await _authService.ChangeUserLanguage(userIdToUpdate, languageId, token);

            return Ok(result);
        }
        [HttpGet("me")]
        [SwaggerOperation(Summary = "[ALL] Get detail current profile")]
        public async Task<IActionResult> GetAccountById()
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            return Ok(await _accountService.GetAccountById(accountId));

        }

        [HttpPut("changePassword")]
        [SwaggerOperation(Summary = "[Operator] Change password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var email = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email)?.Value;

            if (email == null)
            {
                throw new ForbiddenException();
            }

            var task = _accountService.ChangePassword(email, changePasswordDto);
            if (await task)
            {
                return Ok("Update Successfully!");
            }
            else
            {
                throw new BadRequestException("Update False!");
            }
        }
    }
}
