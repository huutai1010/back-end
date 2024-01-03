using AutoMapper;
using AutoMapper.QueryableExtensions;

using Common.Constants;
using Common.Interfaces;
using Common.Models;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookingRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper= mapper;
            _redisCacheService= redisCacheService;
        }

        public async Task<PagedResult<T>> GetAllBookingAsync<T>(QueryParameters queryParameters)
        {
            List<Booking>? items = await _context.Bookings
                .Include(x => x.Account)
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place)
                .IgnoreQueryFilters()
                .OrderByDescending(x => x.CreateTime)
                .ToListAsync();

            var result = _mapper.Map<List<T>>(items);

            string? searchByPropName = queryParameters.SearchBy;
            string? searchText = queryParameters.Search;
            if (!string.IsNullOrEmpty(searchByPropName) && !string.IsNullOrEmpty(searchText))
            {
                var searchProp = typeof(T).GetProperty(searchByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var search = queryParameters.Search?.ToLower();
                if (searchProp != null && !string.IsNullOrEmpty(search))
                {
                    result = result.Where(x =>
                        searchProp.GetValue(x, null)
                        ?.ToString()?.ToLower()
                        .Contains(search) ?? true)
                        .ToList();
                }
            }

            var totalSize = items.Count;
            var pageCount = (double)totalSize / queryParameters.PageSize;
            return new PagedResult<T>
            {
                Data = result
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }
    
        public async Task<Booking> GetBookingDetailAsync(int bookingId)
        {
            var result = await _context.Bookings
                .Include(x => x.Account)
                .ThenInclude(x => x.NationalCodeNavigation)
                .Include(x => x.Transactions)
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place)
                .ThenInclude(x => x.PlaceCategories)
                .SingleOrDefaultAsync(x => x.Id == bookingId);
            if (result == null)
            {
                throw new DllNotFoundException("booking id not found!");
            }

            return result;
        }

        public async Task<List<Booking>> GetTopCustomerOrder()
        {
            var result = _context.Bookings
                .Include(x => x.Account)
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place)
                .OrderByDescending(x => x.CreateTime)
                .Take(4)
                .ToListAsync();

            return await result;
        }

        public async Task<Booking> FindBookingByIdAsync(int bookingId)
        {
            var entity = _context.Bookings.Where(x => x.Id == bookingId)
                .FirstAsync();
            return await entity;
        }

        public async Task<Booking> GetInformationBookingDetail(int bookingId, string languageCode)
        {
            var entity = _context.Bookings
                .Include(x => x.Transactions).ThenInclude(x => x.TransactionDetails)
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceImages.Where(x => x.IsPrimary == true))
                .IgnoreQueryFilters().Where(x => x.Id == bookingId)
                .FirstOrDefaultAsync();
            return await entity;
        }

        public async Task<PagedResult<Booking>> GetHistoryBooking(QueryParameters queryParameters, int accountId, string languageCode)
        {
            var items = await _context.Bookings
                .Include(q => q.BookingPlaces)
                .ThenInclude(a => a.Place)
                .ThenInclude(y => y.PlaceImages.Where(z => z.IsPrimary == true))
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();


            var totalSize = items.Count;
            var pageCount = (double)totalSize / queryParameters.PageSize;
            return new PagedResult<Booking>
            {
                Data = items
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }



        public async Task<List<Booking>> GetHistoryBookingPlace(bool isJourney, string languageCode, int accountId)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            var entity = _context.Bookings
                .Include(q => q.BookingPlaces.Where(x => x.IsJourney == isJourney)).ThenInclude(y => y.Place)
               .ThenInclude(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceImages.Where(x => x.IsPrimary == true))
                .Include(x => x.BookingPlaces)
                .ThenInclude(x => x.Place).ThenInclude(q => q.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
                .IgnoreQueryFilters()
                .OrderByDescending(x => x.Id)
                .Where(x => x.AccountId == accountId && (x.Status != (int)BookingStatus.Cancel && x.Status != (int)BookingStatus.ToPay))
                .ToListAsync();
            return await entity;
        }
        public async Task<List<Booking>> GetHistoryJourney(int accountId)
        {
            var entity = _context.Bookings.Where(x => x.AccountId == accountId)
                .Include(x=>x.BookingPlaces)
                .ToListAsync();
            return await entity;
        }

        public async Task<bool> IsBookingExist(int bookingId)
        {
            var booking = await _context.Bookings.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                return false;
            }
            return true;
        }
    }
}
