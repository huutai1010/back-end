using AutoMapper;
using Common.Interfaces;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ItineraryRepository : BaseRepository<Itinerary>, IItineraryRepository
    {
        private readonly AppDbContext _context;

        public ItineraryRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<int> GetTotalPlaceInTour(int ItineraryId)
        {
            var totalPlace = _context.ItineraryPlaces.CountAsync(x => x.ItineraryId == ItineraryId);
            return await totalPlace;
        }

        public async Task<Itinerary> GetDetailAsync(int ItineraryId)
        {
            var tour = await _context.Itineraries
                .Include(x => x.FeedBacks)
                .ThenInclude(x => x.Account)
                .Include(x => x.CreateBy)
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .Include(x => x.TourDescriptions)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == ItineraryId);
            return tour;
        }

        public async Task<Itinerary> CreateTourAsync(Itinerary tour)
        {
            try
            {
                // Create Tour
                var task = await _context.Itineraries.AddAsync(tour);
                await _context.SaveChangesAsync();

                return task.Entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Itinerary> UpdateTourAsync(Itinerary tour)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // update tour desc
                    var tourDesc = await _context.ItineraryDescriptions.Where(x => x.ItineraryId == tour.Id).IgnoreQueryFilters().ToListAsync();
                    if (tourDesc != null)
                    {
                        _context.ItineraryDescriptions.RemoveRange(tourDesc);
                    }
                    await _context.ItineraryDescriptions.AddRangeAsync(tour.TourDescriptions);

                    // update tour detail
                    var tourDetail = await _context.ItineraryPlaces.Where(x => x.ItineraryId == tour.Id).ToListAsync();
                    if (tourDetail != null)
                    {
                        _context.ItineraryPlaces.RemoveRange(tourDetail);
                    }
                    await _context.ItineraryPlaces.AddRangeAsync(tour.TourDetails);

                    // update tour
                    //tour.CreateTime =  _context.Tours.SingleOrDefaultAsync(x => x.Id == tour.Id).Result.CreateTime;
                    _context.Entry(tour).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message, ex);
                }
            }
            return tour;
        }

        public async Task<bool> ChangeStatusAsync(int ItineraryId)
        {
            bool check = true;
            var tour = await _context.Itineraries.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == ItineraryId);
            if (tour == null)
            {
                check = false;
            }
            else if (tour.Status == 1)
            {
                tour.Status = 0;
            }
            else
            {
                tour.Status = 1;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<bool> IsLanguageCodeExist(string languageCode)
        {
            var check = _context.ConfigLanguages.FirstOrDefaultAsync(x => x.LanguageCode == languageCode);
            if (await check == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsPlaceIDExist(int placeId)
        {
            var check = _context.Places.FirstOrDefaultAsync(x => x.Id == placeId);
            if (await check == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsValidCreateTourId(int accountId)
        {
            bool check = true;
            var task = await _context.Accounts.Where(x => x.Status != 0 && x.RoleId == 2).FirstOrDefaultAsync(x => x.Id == accountId);
            if (task == null)
            {
                return false;
            }
            return check;
        }

        public async Task<List<Itinerary>> GetToursAsync(string languageCode, int topCount)
        {
            return await _context.Itineraries
                .Include(q => q.TourDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(q => q.FeedBacks)
                .OrderByDescending(o => o.Rate)
                .Take(topCount)
                .Where(q => q.TourDescriptions.Any(x => x.LanguageCode == languageCode))
                .ToListAsync();
        }

        public async Task<Itinerary> GetTourDetailByLanguageId(int ItineraryId, string languageCode)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            var tour = await _context.Itineraries
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceImages.Where(x => x.IsPrimary))
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(x => x.FeedBacks.Where(x => x.IsPlace == false).OrderByDescending(x => x.CreateTime).Take(10)).ThenInclude(x => x.Account).ThenInclude(x=>x.NationalCodeNavigation)
                .Include(x => x.TourDescriptions.Where(x => x.LanguageCode == languageCode))
                .FirstOrDefaultAsync(x => x.Id == ItineraryId);
            return tour;
        }

        public async Task<Itinerary> GetRateTour(int? ItineraryId)
        {
            var tourRate = await _context.Itineraries.Where(x => x.Id == ItineraryId && x.Status != 0).FirstAsync();
            return tourRate;
        }

        public async Task<Itinerary> GetBookingTour(int ItineraryId)
        {
            var result = await _context.Itineraries.Include(q => q.TourDetails).FirstOrDefaultAsync(x => x.Id == ItineraryId);

            return result;
        }

        public void DetachedTourInstance(Itinerary tour)
        {
            _context.Entry(tour).State = EntityState.Detached;
        }
        public async Task<Itinerary> GetDetailByIdAsync(int ItineraryId, string languageCode)
        {
            var tour = await _context.Itineraries
                .Include(x => x.TourDescriptions.Where(q => q.LanguageCode == languageCode ))
                .SingleOrDefaultAsync(x => x.Id == ItineraryId);
            return tour;
        }

        public async Task<bool> IsTourExist(int ItineraryId)
        {
            var tour = await _context.Itineraries.IgnoreQueryFilters().SingleOrDefaultAsync(x =>x.Id == ItineraryId);
            if (tour == null)
            {
                return false;
            }
            return true;
        }
    }
}
