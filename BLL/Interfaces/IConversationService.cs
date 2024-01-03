
using AgoraIO.Media;

using BLL.DTOs.Conversation;
using BLL.Responses;

using Common.Models;

namespace BLL.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationListResponse<PagedResult<ConversationListDto>>> GetConversations(QueryParameters queryParameters, int userId);

        Task<ConversationResponse<VideoCallDto>> GenerateVideoCall(int callerId, int receiverId, RtcTokenBuilder2.Role callerRole);
        Task PostConversation(AddConversationDto conversationDto);
        Task SendChatMessage(string fromUsername, string toUsername, string content);
        Task<ConversationListDto> GetConversation(string accountOneUsername, string accountTwoUsername);
    }
}
