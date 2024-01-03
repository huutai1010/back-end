

using BLL.DTOs.Account;
using BLL.DTOs.Feedback;
using BLL.DTOs.Language;
using BLL.Responses;

using Common.Models;

namespace BLL.Interfaces
{
    public interface IFeedbackService
    {
        public Task<FeedbackListResponse<PagedResult<FeedbackListDto>>> GetFeedbacksAsync(QueryParameters queryParameters, int id, bool isplace);

        Task PostFeedbacks(AddFeedbackDto addFeedbackDto, int userId);
        Task<FeedbackResponse<FeedbackListDto>> PutFeedback(int id, FeedbackListDto feedbackListDto);
        Task<FeedbackResponse<FeedbackListDto>> DeleteFeedback(int id);
        Task<bool> ChangeStatusFeedback(int feedbackId);
        Task<FeedbackResponse<FeedbackDetailDto>> GetFeedbackDetailAsync(int id);
        Task<FeedbackListResponse<PagedResult<FeedbackListViewDto>>> GetFeedbackViewList(QueryParameters queryParameters);
        Task<FeedbackListResponse<PagedResult<FeedbacksDto>>> GetFeedbackDto(QueryParameters queryParameters, int id, bool isPlace = false);
    }
}
