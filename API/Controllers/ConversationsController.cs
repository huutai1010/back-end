using AgoraIO.Media;
using BLL.DTOs.Conversation;
using BLL.Exceptions;
using BLL.Interfaces;
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
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }


        [HttpGet]
        [SwaggerOperation(Summary = "[Visitor] Get Conversations", Description = "[Visitor] Get Conversations", OperationId = "getConversations")]
        public async Task<ActionResult<PagedResult<ConversationListDto>>> GetConversations([FromQuery] QueryParameters queryParameters)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (!isValid || userId <= 0)
            {
                throw new ForbiddenException();
            }
            var result = await _conversationService.GetConversations(queryParameters, userId);

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "[Visitor] Create a Conversation", Description = "[Visitor] Create a Conversation", OperationId = "postConversations")]

        public async Task<ActionResult> PostConversation(AddConversationDto conversationDto)
        {
            await _conversationService.PostConversation(conversationDto);

            return NoContent();
        }

        [HttpPost("chat")]
        public async Task<IActionResult> SendChatMessage([FromBody]ChatMessageDto chatMessage)
        {
            await _conversationService.SendChatMessage(chatMessage.FromUsername, chatMessage.ToUsername, chatMessage.Content);
            return NoContent();
        }

        [HttpGet("call/{receiverId:int}/{userRole}")]
        [SwaggerOperation(Summary = "[Visitor] Get video call details", Description = "[Visitor] Get video call details (channel id)", OperationId = "getConversationsCall")]

        public async Task<ActionResult<VideoCallDto>> GetCallingToken([FromRoute] int receiverId, [FromRoute] RtcTokenBuilder2.Role userRole)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (!isValid || userId <= 0)
            {
                throw new ForbiddenException();
            }

            var result = await _conversationService.GenerateVideoCall(userId, receiverId, userRole);
            return Ok(result);
        }

        [HttpGet("{accountOneUsername}/to/{accountTwoUsername}")]
        public async Task<IActionResult> GetConversation([FromRoute]string accountOneUsername, [FromRoute]string accountTwoUsername)
        {
            var result = await _conversationService.GetConversation(accountOneUsername, accountTwoUsername);
            return Ok(result);
        }

    }
}
