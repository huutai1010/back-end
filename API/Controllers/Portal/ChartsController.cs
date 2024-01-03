using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class ChartsController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly IBookingService _bookingService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;

        public ChartsController(IChartService chartService, IBookingService bookingService, ILanguageService languageService, ICurrencyService currencyService)
        {
            _chartService = chartService;
            _bookingService = bookingService;
            _languageService = languageService;
            _currencyService = currencyService;
        }

        [HttpGet("top/place")]
        [SwaggerOperation(Summary = "[Operator] Get top place booking")]
        public async Task<IActionResult> GetTopPlaceBooking([FromQuery] ChartRangeDto chartRange)
        {
            var response = _chartService.GetTopPlaceBooking(chartRange);
            return Ok(response);
        }

        [HttpGet("statictical")]
        [SwaggerOperation(Summary = "[Operator] Get statictical")]
        public async Task<IActionResult> GetChartForOperation([FromQuery] ChartRangeDto chartRange)
        {
            var chartResult = _chartService.GetChartStaticticalAsync(chartRange);
            return Ok(await chartResult);
        }

        [HttpGet("statictical/admin")]
        [SwaggerOperation(Summary = "[Admin] Get statictical")]
        public async Task<IActionResult> GetChartTableStatictical([FromQuery] ChartRangeDto chartRange)
        {
            var chartResult = _chartService.GetTableStatictical(chartRange);
            return Ok(await chartResult);
        }

        [HttpGet("order")]
        [SwaggerOperation(Summary = "[Operator] Get top 4 order sort by date")]
        public async Task<IActionResult> GetTopOrder()
        {
            var result = _bookingService.GetOrderCustomerAsync();
            return Ok(await result);
        }

        [HttpGet("language")]
        [SwaggerOperation(Summary = "[Operator] Get language statictical")]
        public async Task<IActionResult> GetLanguageStatictical([FromQuery]ChartRangeDto chartRange)
        {
            var result = _languageService.GetLanguageStatictical();
            return Ok(await result);
        }

        [HttpGet("national")]
        [SwaggerOperation(Summary = "[Admin] Get national statictical")]
        public async Task<IActionResult> GetNationalStatictical()
        {
            var result = _chartService.GetNationalStatictical();
            return Ok(await result);
        }

        [HttpGet("booking")]
        [SwaggerOperation(Summary = "[Operator] Get chart revenue", Description = "options to get chart:  7, 1, 3")]
        public async Task<IActionResult> GetChartRevenueForOperator(int options, [FromQuery] ChartRangeDto chartRange)
        {
            var chart = await _chartService.GetChartRevenue(options, chartRange);
            return Ok(chart);
        }

        [HttpGet("user")]
        [SwaggerOperation(Summary = "[Admin] Get chart user register", Description = "options to get chart: 3, 6, 1")]
        public async Task<IActionResult> GetChartUserForOperator(int options, [FromQuery] ChartRangeDto chartRange)
        {
            var chart =await _chartService.GetChartUser(options, chartRange);
            return Ok(chart);
        }

        [HttpGet("/api/portal/exchanges/load")]
        [AllowAnonymous]
        public async Task<IActionResult> LoadExchanges()
        {
            await _currencyService.LoadExchanges();
            return Ok();
        }

        [HttpGet("/api/portal/dashboard/load")]
        [AllowAnonymous]
        public async Task<IActionResult> LoadDashboard()
        {
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today;
            await _chartService.ReloadDashboard(fromDate, toDate);
            return Ok();
        }
    }
}
