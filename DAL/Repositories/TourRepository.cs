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
    public class TourRepository : BaseRepository<Tour>, ITourRepository
    {
        private readonly AppDbContext _context;

        public TourRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<int> GetTotalPlaceInTour(int tourId)
        {
            var totalPlace = _context.TourDetails.CountAsync(x => x.TourId == tourId);
            return await totalPlace;
        }

        public async Task<Tour> GetDetailAsync(int tourId)
        {
            var tour = await _context.Tours
                .Include(x => x.FeedBacks)
                .ThenInclude(x => x.Account)
                .Include(x => x.CreateBy)
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .Include(x => x.TourDescriptions)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == tourId);
            return tour;
        }

        public async Task<Tour> CreateTourAsync(Tour tour)
        {
            try
            {
                // Create Tour
                var task = await _context.Tours.AddAsync(tour);
                await _context.SaveChangesAsync();

                return task.Entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Tour> UpdateTourAsync(Tour tour)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // update tour desc
                    var tourDesc = await _context.TourDescriptions.Where(x => x.TourId == tour.Id).ToListAsync();
                    if (tourDesc != null)
                    {
                        _context.TourDescriptions.RemoveRange(tourDesc);
                    }
                    await _context.TourDescriptions.AddRangeAsync(tour.TourDescriptions);

                    // update tour detail
                    var tourDetail = await _context.TourDetails.Where(x => x.TourId == tour.Id).ToListAsync();
                    if (tourDetail != null)
                    {
                        _context.TourDetails.RemoveRange(tourDetail);
                    }
                    await _context.TourDetails.AddRangeAsync(tour.TourDetails);

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

        public async Task<bool> ChangeStatusAsync(int tourId)
        {
            bool check = true;
            var tour = await _context.Tours.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == tourId);
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

        public async Task<List<Tour>> GetToursAsync(string languageCode, int topCount)
        {
            return await _context.Tours
                .Include(q => q.TourDescriptions.Where(x => x.LanguageCode == languageCode))
                .OrderByDescending(o => o.Rate)
                .Take(topCount)
                .Where(q => q.TourDescriptions.Any(x => x.LanguageCode == languageCode))
                .ToListAsync();
        }

        public async Task<Tour> GetTourDetailByLanguageId(int tourId, string languageCode)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            var tour = await _context.Tours
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceImages.Where(x => x.IsPrimary))
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
                .Include(x => x.TourDetails)
                .ThenInclude(x => x.Place)
                .ThenInclude(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(x => x.FeedBacks.Where(x => x.IsPlace == false).OrderByDescending(x => x.CreateTime).Take(2)).ThenInclude(x => x.Account).ThenInclude(x=>x.NationalCodeNavigation)
                .Include(x => x.TourDescriptions.Where(x => x.LanguageCode == languageCode))
                .FirstOrDefaultAsync(x => x.Id == tourId);
            return tour;
        }

        public async Task<Tour> GetRateTour(int? tourId)
        {
            var tourRate = await _context.Tours.Where(x => x.Id == tourId && x.Status != 0).FirstAsync();
            return tourRate;
        }

        public async Task<Tour> GetBookingTour(int tourId)
        {
            var result = await _context.Tours.Include(q => q.TourDetails).FirstOrDefaultAsync(x => x.Id == tourId);

            return result;
        }

        public void DetachedTourInstance(Tour tour)
        {
            _context.Entry(tour).State = EntityState.Detached;
        }
        public async Task<Tour> GetDetailByIdAsync(int tourId, string languageCode)
        {
            var tour = await _context.Tours
                .Include(x => x.TourDescriptions.Where(q => q.LanguageCode == languageCode ))
                .SingleOrDefaultAsync(x => x.Id == tourId);
            return tour;
        }

        public async Task<bool> IsTourExist(int tourId)
        {
            var tour = await _context.Tours.IgnoreQueryFilters().SingleOrDefaultAsync(x =>x.Id == tourId);
            if (tour == null)
            {
                return false;
            }
            return true;
        }
    }
}
