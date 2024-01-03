using Common.Models.Chart;
using DAL.DatabaseContext;
using DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ChartRepository : IChartRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public ChartRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> GetTotalUser(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Accounts.Where(x => x.RoleId == 3 && x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalCategory(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Categories.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalLanguage(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.ConfigLanguages.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalOrder(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Bookings.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal> GetTotalRevenue(DateTime startTime, DateTime endTime)
        {
            try
            {
                decimal result = 0;
                result = await _context.Bookings.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).SumAsync(x => x.Total);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalPlace(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Places.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalTour(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Itineraries.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).CountAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalTransactionComplete(DateTime startTime, DateTime endTime)
        {
            try
            {
                int result = 0;
                result = await _context.Transactions.Where(x => x.CreateTime >= startTime && x.CreateTime <= endTime).Where(x => x.Status == 2).CountAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalPlaceInCurrentMonth()
        {
            try
            {
                int result = 0;

                // Get the current month and year
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;
                int currentMonth = currentDate.Month;

                // Calculate the start and end dates for the current month
                DateTime startDate = new DateTime(currentYear, currentMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                result = await _context.Places
                    .Where(place => place.CreateTime >= startDate && place.CreateTime <= endDate)
                    .CountAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalUserInCurrentMonth()
        {
            try
            {
                int result = 0;

                // Get the current month and year
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;
                int currentMonth = currentDate.Month;

                // Calculate the start and end dates for the current month
                DateTime startDate = new DateTime(currentYear, currentMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                result = await _context.Accounts
                    .Where(account => account.CreateTime >= startDate && account.CreateTime <= endDate)
                    .CountAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalBookingInCurrentMonth()
        {
            try
            {
                int result = 0;

                // Get the current month and year
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;
                int currentMonth = currentDate.Month;

                // Calculate the start and end dates for the current month
                DateTime startDate = new DateTime(currentYear, currentMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                result = await _context.Bookings
                    .Where(x => x.CreateTime >= startDate && x.CreateTime <= endDate)
                    .CountAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalTransactionSuccessInCurrentMonth()
        {
            try
            {
                int result = 0;

                // Get the current month and year
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;
                int currentMonth = currentDate.Month;

                // Calculate the start and end dates for the current month
                DateTime startDate = new DateTime(currentYear, currentMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                result = await _context.Transactions
                    .Where(x => x.CreateTime >= startDate && x.CreateTime <= endDate && x.Status == 2)
                    .CountAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalTourInCurrentMonth()
        {
            try
            {
                int result = 0;

                // Get the current month and year
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;
                int currentMonth = currentDate.Month;

                // Calculate the start and end dates for the current month
                DateTime startDate = new DateTime(currentYear, currentMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                result = await _context.Itineraries
                    .Where(tour => tour.CreateTime >= startDate && tour.CreateTime <= endDate)
                    .CountAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ChartRevenueModel> GetChartRevenueV2(DateTime startDate, DateTime endDate)
        {
            var chartRevenueModels = _context.Bookings
                       .Where(o => o.CreateTime >= startDate && o.CreateTime <= endDate)
                       .Select(o => new
                       {
                           Date = o.CreateTime.Date, // Project the date part of CreateTime
                           Total = o.Total
                       })
                       .GroupBy(o => o.Date)
                       .Select(g => new ChartRevenueModel
                       {
                           Date = g.Key.ToString("yyyy-MM-dd"),
                           TotalPrice = g.Sum(o => o.Total),
                           TotalBooking = g.Count()
                       })
                       .IgnoreQueryFilters()
                       .ToList();

            return chartRevenueModels;
        }

        public List<ChartUserModel> GetChartUser(DateTime startDate, DateTime endDate)
        {
            var chartRevenueModels = _context.Accounts
                       .Where(o => o.CreateTime >= startDate && o.CreateTime <= endDate && o.RoleId == 3)
                       .Select(o => new
                       {
                           Date = o.CreateTime.Date, // Project the date part of CreateTime
                       })
                       .GroupBy(o => o.Date)
                       .Select(g => new ChartUserModel
                       {
                           Date = g.Key.ToString("yyyy-MM-dd"),
                           TotalUser = g.Count()
                       })
                       .IgnoreQueryFilters()
                       .ToList();

            return chartRevenueModels;
        }

        public List<ChartUserModel> GetChartUserV2(DateTime startDate, DateTime endDate)
        {
            var chartRevenueModels = _context.Accounts
                       .Where(o => o.CreateTime >= startDate && o.CreateTime <= endDate && o.RoleId == 3)
                       .Select(o => new
                       {
                           Year = o.CreateTime.Year,
                           Month = o.CreateTime.Month,
                       })
                       .GroupBy(o => new { Year = o.Year, Month = o.Month })
                       .Select(g => new ChartUserModel
                       {
                           Date = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM-dd"),
                           TotalUser = g.Count()
                       })
                       .IgnoreQueryFilters()
                       .ToList();

            return chartRevenueModels;
        }

        public List<TopPlaceBookingModel> GetTopPlaceBooking(DateTime startTime, DateTime endTime)
        {
            var topPlaceBookingModel = _context.BookingPlaces
                        .Where(bp => bp.Booking.CreateTime >= startTime && bp.Booking.CreateTime <= endTime)
                        .GroupBy(bp => bp.PlaceId)
                        .OrderByDescending(group => group.Count())
                        .Take(10)
                        .Select((bpGroup) => new TopPlaceBookingModel
                        {
                            PlaceId = bpGroup.Key,
                            TotalBooking = bpGroup.Count(),
                            PlaceName = _context.Places
                                .Where(place => place.Id == bpGroup.Key)
                                .Select(place => place.Name)
                                .FirstOrDefault()
                        })
                        .ToList();

            return topPlaceBookingModel;
        }
    }
}
