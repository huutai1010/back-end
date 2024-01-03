using BLL.DTOs.Account;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace API.Controllers.Portal
{
    [Authorize(Roles = "1,2")]
    [Route("api/portal/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
      
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPut("profile")]
        [SwaggerOperation(Summary = "[Operator] update profile")]
        public async Task<IActionResult> UpdateOpeationProfile([FromBody]AccountUpdateDto opUpdateDto)
        {
            var email = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email)?.Value;

            if (email == null)
            {
                throw new ForbiddenException();
            }

            var task = _accountService.UpdateOpAccount(email, opUpdateDto);
            if (await task)
            {
                return Ok("Update Profile Successfully!");
            }
            else
            {
                throw new BadRequestException("Update Profile False!");
            }
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
