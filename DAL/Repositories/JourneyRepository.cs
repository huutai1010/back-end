using AutoMapper;
using Common.Interfaces;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class JourneyRepository : BaseRepository<Journey>, IJourneyRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public JourneyRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService cacheService) : base(context, mapper, unitOfWork, cacheService)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Journey>> GetJourneyByBookingId(int status, List<int> journeyIds, string languageCode)
        {
            List<Journey> result;
            if (status >= 2) {
                result = await _context.Journeys
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceDescriptions.Where(pd => pd.LanguageCode == languageCode))
                .IgnoreQueryFilters()
                .Where(x => x.Status >= status && journeyIds.Contains(x.Id))
                .ToListAsync();
            }
            else
            {
                result =  await _context.Journeys
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceDescriptions.Where(pd => pd.LanguageCode == languageCode))
                .IgnoreQueryFilters()
                .Where(x => x.Status == status && journeyIds.Contains(x.Id))
                .ToListAsync();
            }
            return result.OrderByDescending(x => x.Id).ToList();

        }
        public async Task<Journey> PostJourney(Journey journeyDto)
        {
            try
            {
                var task = await _context.Journeys.AddAsync(journeyDto);
                await _context.SaveChangesAsync();

                return task.Entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> PutJourneyStatus(int journeyId, int status)
        {
            var entity = await _context.Journeys.Where(x => x.Id == journeyId).IgnoreQueryFilters().FirstAsync();
            if (entity == null)
            {
                return false;
            }
            else
            {
                entity.Status = status;
            }
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Journey> FindJourneyById(int journeyId)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            var entity = _context.Journeys
                .Include(x => x.BookingPlaces.OrderBy(x=>x.Ordinal))
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceImages.Where(x => x.IsPrimary == true))
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
                .IgnoreQueryFilters()
                .Where(x => x.Id == journeyId)
                .FirstAsync();
            return await entity;
        }
    }
}
