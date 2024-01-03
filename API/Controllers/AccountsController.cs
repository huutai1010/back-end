using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.DTOs.Place.MarkPlace;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Common.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all accounts (demonstration purpose)")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccounts([FromQuery] QueryParameters queryParameters)
        {
            var result = await _accountService.GetAccountsAsync(queryParameters);

            return Ok(result);
        }

        [HttpGet("nearby")]
        [SwaggerOperation(Summary = "Get all accounts nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccountsNearby()
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _accountService.GetAccountsNearby(accountId, languageCode);
            return Ok(result);
        }


        [HttpPost("/notification")]
        [SwaggerOperation(Summary = "[Visitor] Push Notification", Description = "1 - Send add friend request / 2 - Request accepted / 3 - Payment success / 4 - Send call request", OperationId = "PushNotification")]
        public async Task<ActionResult> SendNotification(int notificationType, int receiverId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int senderId);
            if (!isValid || senderId <= 0)
            {
                throw new ForbiddenException();
            }
            string response = await _accountService.SendNotification(senderId, receiverId, null, notificationType);
            return Ok(new
            {
                NotificationId = response,
            });
        }

        [HttpGet("mark-place")]
        [SwaggerOperation(Summary = "[Visitor] Get places marked as favorite of user logging in.")]
        public async Task<IActionResult> GetMarkPlaceByAccountId([FromQuery] QueryParameters queryParameters)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var response = await _accountService.GetMarkPlaceWithAccountAsync(queryParameters, accountId);
            return Ok(response);
        }
        [HttpPost("mark-place/{placeId}")]
        [SwaggerOperation(Summary = "[Visitor] Mark/Remove places as favorite.")]
        public async Task<IActionResult> PostMarkPlaceByAccountId([FromRoute] int placeId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var task = await _accountService.PostMarkPlaceWithAccountAsync(accountId, placeId);
            return Ok(new StatusResposne("Action successfully!"));
        }
    }
}
