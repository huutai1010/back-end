using Common.Models;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IFeedbackRepository : IBaseRepository<FeedBack> 
    {
        Task<PagedResult<FeedbackListDto>> GetFeedbackAsync<FeedbackListDto>(QueryParameters queryParameters, int id , bool isplace);
        Task<List<double>> GetFeedbackAvg(int tagetId, bool isPlace);
        Task<bool> ChangeStatusFeedback(int feedbackId);
        Task<bool> IsExistFeedback(int feedbackId);
        Task<FeedBack> GetFeedbackDetailById(int id);
    }
}
