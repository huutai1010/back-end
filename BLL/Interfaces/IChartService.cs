using BLL.DTOs;
using BLL.DTOs.Chart;
using BLL.DTOs.Nationality;
using BLL.Responses;
using Common.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IChartService
    {
        Task<ChartResponse<List<ChartDto>>> GetChartStaticticalAsync(ChartRangeDto chartRange);
        Task<ChartResponse<List<ChartDto>>> GetTableStatictical([FromQuery] ChartRangeDto chartRange);
        Task<StaticticalNationalResponse<List<NationalityChartDto>>> GetNationalStatictical();
        Task<ChartListResponse<List<ChartRevenueModel>>> GetChartRevenue(DateTime startTime, DateTime endTime);
        Task<ChartListResponse<List<ChartRevenueModel>>> GetChartRevenue(int chartOptions, ChartRangeDto chartRange);
        Task<ChartListResponse<List<ChartUserModel>>> GetChartUser(int chartOptions, ChartRangeDto chartRange);
        Task<ChartListResponse<List<ChartUserModel>>> GetChartUser(DateTime startTime, DateTime endTime);
        ChartListResponse<List<TopPlaceBookingModel>> GetTopPlaceBooking(ChartRangeDto chartRange);
        Task<ChartListResponse<List<ChartUserModel>>> GetChartUserV2(DateTime startTime, DateTime endTime);
        Task ReloadDashboard(DateTime from, DateTime to);
    }
}
