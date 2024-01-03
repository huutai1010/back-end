using Common.Models;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
        Task<PagedResult<ConversationListDto>> GetConversationsAsync<ConversationListDto>(QueryParameters queryParameters, int userId);
        Task<bool> IsConversationExist(int AccountOne, int AccountTwo);
        Task<string> GetChannelToken(int AccountOne, int AccountTwo);
        Task<Conversation> GetConversation(string accountOneUsername, string accountTwoUsername);
    }
}
