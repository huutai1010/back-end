using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IItineraryRepository : IBaseRepository<Itinerary>
    {
        Task<int> GetTotalPlaceInTour(int tourId);
        Task<Itinerary> GetDetailAsync(int tourId);
        Task<bool> ChangeStatusAsync(int tourId);
        Task<Itinerary> CreateTourAsync(Itinerary tour);
        Task<Itinerary> UpdateTourAsync(Itinerary tour);
        Task<bool> IsPlaceIDExist(int placeId);
        Task<bool> IsLanguageCodeExist(string languageCode);
        Task<bool> IsValidCreateTourId(int accountId);
        Task<List<Itinerary>> GetToursAsync(string languageCode, int topCount);
        Task<Itinerary> GetTourDetailByLanguageId(int tourId, string languageCode);
        Task<Itinerary> GetRateTour(int? tourId);
        Task<bool> IsTourExist(int tourId);

        Task<Itinerary> GetBookingTour(int tourId);
        void DetachedTourInstance(Itinerary tour);
        Task<Itinerary> GetDetailByIdAsync(int tourId, string languageCode);
    }
}
