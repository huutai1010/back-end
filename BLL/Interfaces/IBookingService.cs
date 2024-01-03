using BLL.DTOs.Booking;
using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Place.CelebratedPlace;
using BLL.DTOs.Transaction;
using BLL.Responses;

using Common.Models;
using Common.Models.Paypal;

using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces
{
    public interface IBookingService
    {
        Task<PlaceListResponse<List<CelebratedPlaceDto>>> GetCelebratedPlace(int journeyId, string languageCode);
        Task PostCelebratedPlace(int bookingDetailId, List<IFormFile> imageFiles);
        Task<CheckInPlaceResultDto> CheckInPlace(int bookingDetailId, bool isFinish);
        Task<BookingListResponse<PagedResult<BookingListDto>>> GetListAsync(QueryParameters parameters);
        Task<BookingResponse<BookingDetailDto>> GetDetailAsync(int bookingId);
        Task<OrderStaticticalResponse<List<OrderViewOpDto>>> GetOrderCustomerAsync();
        Task<BookingListResponse<PagedResult<HistoryBookingViewDto>>> GetHistoryBooking(QueryParameters queryParameters, int accountId, string languageCode);
        Task<BookingPlaceListResponse<List<JourneyPlaceViewDto>>> GetHistoryBookingPlace(bool isJourney, string languageCode, int accountId);
        Task<Link> CreateBookingAsync(int userId, PostBookingDto bookingDto);
        Task<BookingResponse<BookingViewDto>> GetInformationBookingDetail(int bookingId, string languageCode);
        Task CancelBooking(int bookingId);
        Task<object> ConfirmBooking(string paymentId, string paymentAccountId);
        Task CancelBatchBooking();
    }
}
