
using DAL.Entities;
namespace DAL.Interfaces
{
    public interface IBookingPlaceRepository : IBaseRepository<BookingPlace>
    {
        Task<List<BookingPlace>> GetCelebratedPlaceById(int bookingId, string languageCode);
    }
}
