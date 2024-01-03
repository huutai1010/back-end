using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Chart;
using BLL.DTOs.Language;
using BLL.DTOs.Nationality;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Common.Constants;
using Common.Interfaces;
using Common.Models.Chart;

using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BLL.Services
{
    public class ChartService : IChartService
    {
        private readonly IChartRepository _chartRepository;
        private readonly INationalityRepository _nationalityRepository;
        private readonly ILanguageService _languageService;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;

        public ChartService(IChartRepository chartRepository,
            IRedisCacheService redisCacheService,
            INationalityRepository nationalityRepository,
            ILanguageService languageService,
            IMapper mapper
            )
        {
            _chartRepository = chartRepository;
            _redisCacheService = redisCacheService;
            _nationalityRepository = nationalityRepository;
            _languageService = languageService;
            _mapper = mapper;
        }

        public async Task<ChartResponse<List<ChartDto>>> GetTableStatictical(ChartRangeDto chartRange)
        {
            List<ChartDto> chartList = new();

            DateTime startTime = new DateTime(1990, 1, 1);
            DateTime endTime = DateTime.Now;
            if (chartRange.StartTime != null && chartRange.EndTime != null)
            {
                startTime = DateTime.ParseExact(chartRange.StartTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                endTime = DateTime.ParseExact(chartRange.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            for (int i = 1; i <= 4; i++)
            {
                switch (i)
                {
                    case 1:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Users",
                            Total = await _chartRepository.GetTotalUser(startTime, endTime),
                            NumberIncreased = await _chartRepository.GetTotalUserInCurrentMonth()
                        });
                        break;
                    case 2:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Categories",
                            Total = await _chartRepository.GetTotalCategory(startTime, endTime),
                            NumberIncreased = 0
                        });
                        break;
                    case 3:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Languages",
                            Total = await _chartRepository.GetTotalLanguage(startTime, endTime),
                            NumberIncreased = 0
                        });
                        break;
                    case 4:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Revenue",
                            Total = (int)await _chartRepository.GetTotalRevenue(startTime, endTime),
                            NumberIncreased = 0
                        });
                        break;
                }
            }

            return new ChartResponse<List<ChartDto>>(chartList);
        }

        public async Task<StaticticalNationalResponse<List<NationalityChartDto>>> GetNationalStatictical()
        {
            List<NationalityChartDto>? items = await _redisCacheService.Get<List<NationalityChartDto>>(RedisCacheKeys.DASHBOARD_NATIONAL);
            int total = 0;

            if (items == null)
            {
                var nationals = _nationalityRepository.GetListNationalityAsync();

                var dto = _mapper.Map<List<NationalityChartDto>>(await nationals);

                var result = dto.Where(x => x.Quantity != 0).ToList();

                foreach (var item in result)
                {
                    if (item.Quantity != null)
                    {
                        total += (int)item.Quantity;
                    }
                }

                foreach (var item in result)
                {
                    if (item.Quantity.HasValue)
                    {
                        var ratio = ((decimal)item.Quantity / (decimal)total) * 100;
                        item.Ratio = Math.Round(ratio, 1);
                    }
                }
                items = result.OrderByDescending(x => x.Ratio).ToList();
                await _redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.DASHBOARD_NATIONAL, items);
            }
            else
            {
                foreach (var item in items)
                {
                    if (item.Quantity != null)
                    {
                        total += (int)item.Quantity;
                    }
                }
            }

            return new StaticticalNationalResponse<List<NationalityChartDto>>(items, total);
        }

        public async Task<ChartResponse<List<ChartDto>>> GetChartStaticticalAsync(ChartRangeDto chartRange)
        {

            List<ChartDto> chartList = new();

            DateTime startTime = new DateTime(1990, 1, 1);
            DateTime endTime = DateTime.Now;
            if (chartRange.StartTime != null && chartRange.EndTime != null)
            {
                startTime = DateTime.ParseExact(chartRange.StartTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                endTime = DateTime.ParseExact(chartRange.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            for (int i  = 1; i <= 4; i++)
            {
                switch (i)
                {
                    case 1: chartList.Add(new ChartDto {
                        Id = i,
                        Name = "Places",
                        Total = await _chartRepository.GetTotalPlace(startTime, endTime),
                        NumberIncreased = await _chartRepository.GetTotalPlaceInCurrentMonth()
                        });
                        break;
                    case 2:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Tours",
                            Total = await _chartRepository.GetTotalTour(startTime, endTime),
                            NumberIncreased = await _chartRepository.GetTotalTourInCurrentMonth()
                        });
                        break;
                    case 3:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Bookings",
                            Total = await _chartRepository.GetTotalOrder(startTime, endTime),
                            NumberIncreased = await _chartRepository.GetTotalBookingInCurrentMonth()
                        });
                        break;
                    case 4:
                        chartList.Add(new ChartDto
                        {
                            Id = i,
                            Name = "Transactions",
                            Total = await _chartRepository.GetTotalTransactionComplete(startTime, endTime),
                            NumberIncreased = await _chartRepository.GetTotalTransactionSuccessInCurrentMonth()
                        });
                        break;
                }
            }

            return new ChartResponse<List<ChartDto>>(chartList);
        }

        public async Task<ChartListResponse<List<ChartRevenueModel>>> GetChartRevenue(DateTime startTime, DateTime endTime)
        {
            // validate time 
            if (startTime > endTime)
            {
                throw new BadRequestException("Start time cannot be greater than end time");
            }
            List<ChartRevenueModel>? chart = await _redisCacheService.Get<List<ChartRevenueModel>>(RedisCacheKeys.CHART_REVENUE);
            if (chart == null)
            {
                chart = _chartRepository.GetChartRevenueV2(startTime, endTime);
                await _redisCacheService.SaveCacheAsync(RedisCacheKeys.CHART_REVENUE, chart);
            }
            return new ChartListResponse<List<ChartRevenueModel>>(chart);
        }

        public async Task<ChartListResponse<List<ChartUserModel>>> GetChartUser(DateTime startTime, DateTime endTime)
        {
            // validate time 
            if (startTime > endTime)
            {
                throw new BadRequestException("Start time cannot be greater than end time");
            }

            List<ChartUserModel>? chart = await _redisCacheService.Get<List<ChartUserModel>>(RedisCacheKeys.CHART_USER);
            if (chart == null)
            {
                chart = _chartRepository.GetChartUser(startTime, endTime);
                await _redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.CHART_REVENUE, chart);
            }

            return new ChartListResponse<List<ChartUserModel>>(chart);
        }

        public async Task<ChartListResponse<List<ChartRevenueModel>>> GetChartRevenue(int chartOptions, ChartRangeDto chartRange)
        {
            DateTime endTime = DateTime.Now.AddDays(+1);
            DateTime startTime = new();
            string redisCacheKey = "";

            if (chartRange.StartTime != null && chartRange.EndTime != null)
            {
                startTime = DateTime.ParseExact(chartRange.StartTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                endTime = DateTime.ParseExact(chartRange.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<ChartRevenueModel>? chartCustome= _chartRepository.GetChartRevenueV2(startTime, endTime);
                var totalRevenueCustome = chartCustome.Sum(x => x.TotalPrice);
                return new ChartListResponse<List<ChartRevenueModel>>(chartCustome, totalRevenueCustome);
            }

            switch (chartOptions)
            {
                case 7:
                    startTime = endTime.AddDays(-8);
                    redisCacheKey = RedisCacheKeys.CHART_REVENUE_7D;
                    break;

                case 1:
                    startTime = endTime.AddDays(-31);
                    redisCacheKey = RedisCacheKeys.CHART_REVENUE_1M;
                    break;

                case 3:
                    startTime = endTime.AddDays(-91);
                    redisCacheKey = RedisCacheKeys.CHART_REVENUE_3M;
                    break;

                default: throw new BadRequestException("chart optione time is not support!");
            }

            List<ChartRevenueModel>? chart = await _redisCacheService.Get<List<ChartRevenueModel>>(redisCacheKey);
            decimal totalRevenue = 0;
            if (chart == null)
            {
                chart = _chartRepository.GetChartRevenueV2(startTime, endTime);
                await _redisCacheService.SaveCachePermanentAsync(redisCacheKey, chart);
            }
            totalRevenue = chart.Sum(x => x.TotalPrice);
            return new ChartListResponse<List<ChartRevenueModel>>(chart, totalRevenue);
        }

        public async Task<ChartListResponse<List<ChartUserModel>>> GetChartUser(int chartOptions, ChartRangeDto chartRange)
        {
            DateTime endTime = DateTime.Now.AddDays(+1);
            DateTime startTime = new();
            string redisCacheKey = "";

            if (chartRange.StartTime != null && chartRange.EndTime != null)
            {
                startTime = DateTime.ParseExact(chartRange.StartTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                endTime = DateTime.ParseExact(chartRange.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<ChartUserModel>? chartCustome =  _chartRepository.GetChartUserV2(startTime, endTime);
                var totalUserCustome = chartCustome.Sum(x => x.TotalUser);
                return new ChartListResponse<List<ChartUserModel>>(chartCustome, totalUserCustome);
            }

            switch (chartOptions)
            {
                case 3:
                    startTime = new DateTime(endTime.Year, endTime.Month, 1).AddDays(-1).AddMonths(-2);
                    redisCacheKey = RedisCacheKeys.CHART_USER_3M;
                    break;

                case 6:
                    startTime = new DateTime(endTime.Year, endTime.Month, 1).AddDays(-1).AddMonths(-5);
                    redisCacheKey = RedisCacheKeys.CHART_USER_6M;
                    break;

                case 1:
                    startTime = new DateTime(endTime.Year, endTime.Month, 1).AddDays(-1).AddMonths(-11);
                    redisCacheKey = RedisCacheKeys.CHART_USER_1Y;
                    break;

                default: throw new BadRequestException("chart optione time is not support!");
            }

            List<ChartUserModel>? chart = await _redisCacheService.Get<List<ChartUserModel>>(redisCacheKey);
            decimal totalUser = 0;
            if (chart == null)
            {
                chart = _chartRepository.GetChartUserV2(startTime, endTime);
                await _redisCacheService.SaveCachePermanentAsync(redisCacheKey, chart);
            }
            totalUser = chart.Sum(x => x.TotalUser);

            return new ChartListResponse<List<ChartUserModel>>(chart, totalUser);
        }

        public async Task<ChartListResponse<List<ChartUserModel>>> GetChartUserV2(DateTime startTime, DateTime endTime)
        {

            var chart = _chartRepository.GetChartUserV2(startTime, endTime);

            return new ChartListResponse<List<ChartUserModel>>(chart);
        }

        public ChartListResponse<List<TopPlaceBookingModel>> GetTopPlaceBooking(ChartRangeDto chartRange)
        {
            DateTime startTime = new DateTime(1990, 1, 1);
            DateTime endTime = DateTime.Now;
            if (chartRange.StartTime != null && chartRange.EndTime != null)
            {
                startTime = DateTime.ParseExact(chartRange.StartTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                endTime = DateTime.ParseExact(chartRange.EndTime, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            var response = _chartRepository.GetTopPlaceBooking(startTime, endTime);
            return new ChartListResponse<List<TopPlaceBookingModel>>(response);
        }

        public async Task ReloadDashboard(DateTime from, DateTime to)
        {
            var chartRange = new ChartRangeDto();
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_REVENUE_7D);
            await GetChartRevenue(7, chartRange);
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_REVENUE_1M);
            await GetChartRevenue(1, chartRange);
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_REVENUE_3M);
            await GetChartRevenue(3, chartRange);

            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_USER_3M);
            await GetChartUser(3, chartRange);
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_USER_6M);
            await GetChartUser(6, chartRange);
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_USER_1Y);
            await GetChartUser(1, chartRange);
            await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_USER);
            await GetChartUser(from, to);

            await _redisCacheService.RemoveAsync(RedisCacheKeys.DASHBOARD_LAGUAGE);
            await _languageService.GetLanguageStatictical();

            await _redisCacheService.RemoveAsync(RedisCacheKeys.DASHBOARD_NATIONAL);
            await GetNationalStatictical();


        }
    }
}
