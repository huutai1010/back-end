using Common.Models.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IChartRepository
    {
        Task<int> GetTotalOrder(DateTime startTime, DateTime endTime);
        Task<int> GetTotalPlace(DateTime startTime, DateTime endTime);
        Task<int> GetTotalTour(DateTime startTime, DateTime endTime);
        Task<int> GetTotalUser(DateTime startTime, DateTime endTime);
        Task<int> GetTotalLanguage(DateTime startTime, DateTime endTime);
        Task<int> GetTotalCategory(DateTime startTime, DateTime endTime);
        Task<decimal> GetTotalRevenue(DateTime startTime, DateTime endTime);
        Task<int> GetTotalUserInCurrentMonth();
        Task<int> GetTotalTransactionComplete(DateTime startTime, DateTime endTime);
        Task<int> GetTotalPlaceInCurrentMonth();
        Task<int> GetTotalTourInCurrentMonth();
        Task<int> GetTotalTransactionSuccessInCurrentMonth();
        Task<int> GetTotalBookingInCurrentMonth();
        List<ChartRevenueModel> GetChartRevenueV2(DateTime startDate, DateTime endDate);
        List<ChartUserModel> GetChartUser(DateTime startDate, DateTime endDate);
        List<ChartUserModel> GetChartUserV2(DateTime startDate, DateTime endDate);
        List<TopPlaceBookingModel> GetTopPlaceBooking(DateTime startTime, DateTime endTime);
    }
}
