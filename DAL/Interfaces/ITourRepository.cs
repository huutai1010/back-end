using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITourRepository : IBaseRepository<Tour>
    {
        Task<int> GetTotalPlaceInTour(int tourId);
        Task<Tour> GetDetailAsync(int tourId);
        Task<bool> ChangeStatusAsync(int tourId);
        Task<Tour> CreateTourAsync(Tour tour);
        Task<Tour> UpdateTourAsync(Tour tour);
        Task<bool> IsPlaceIDExist(int placeId);
        Task<bool> IsLanguageCodeExist(string languageCode);
        Task<bool> IsValidCreateTourId(int accountId);
        Task<List<Tour>> GetToursAsync(string languageCode, int topCount);
        Task<Tour> GetTourDetailByLanguageId(int tourId, string languageCode);
        Task<Tour> GetRateTour(int? tourId);
        Task<bool> IsTourExist(int tourId);

        Task<Tour> GetBookingTour(int tourId);
        void DetachedTourInstance(Tour tour);
        Task<Tour> GetDetailByIdAsync(int tourId, string languageCode);
    }
}
