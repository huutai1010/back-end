using Common.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<PagedResult<T>> GetAllBookingAsync<T>(QueryParameters queryParameters);
        Task<Booking> GetBookingDetailAsync(int bookingId);
        Task<List<Booking>> GetTopCustomerOrder();
        Task<Booking> FindBookingByIdAsync(int bookingId);
        Task<bool> IsBookingExist(int bookingId);

        Task<Booking> GetInformationBookingDetail(int bookingId, string languageCode);
        Task<PagedResult<Booking>> GetHistoryBooking(QueryParameters queryParameters, int accountId, string languageCode);

        Task<List<Booking>> GetHistoryBookingPlace(bool isJourney, string languageCode, int accountId);
        Task<List<Booking>> GetHistoryJourney(int accountId);
    }
}
