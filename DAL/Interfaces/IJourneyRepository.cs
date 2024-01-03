using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IJourneyRepository : IBaseRepository<Journey>
    {
        Task<List<Journey>> GetJourneyByBookingId( int status, List<int> journeyIds, string languageCode);
        Task<Journey> PostJourney(Journey journeyDto);

        Task<bool> PutJourneyStatus(int journeyId, int status);
        Task<Journey> FindJourneyById(int journeyId);
    }
}
