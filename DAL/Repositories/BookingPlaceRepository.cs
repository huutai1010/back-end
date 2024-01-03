using AutoMapper;
using Common.Interfaces;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BookingPlaceRepository : BaseRepository<BookingPlace>, IBookingPlaceRepository
    {
        private readonly AppDbContext _context;

        public BookingPlaceRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<List<BookingPlace>> GetCelebratedPlaceById(int journeyId, string languageCode)
        {
            var entity = _context.BookingPlaces.Where(x => x.JourneyId == journeyId)
                .Include(q => q.CelebrateImages)
                .Include(q => q.Place).ThenInclude(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .ToListAsync();
            return await entity;
        }
        public async Task<List<CelebrateImage>> GetCelebratedImageById(int bookingDetailId)
        {
            var entity = _context.CelebrateImages.Where(x => x.BookingDetailId == bookingDetailId).ToListAsync();
            return await entity;
        }
    }
}
