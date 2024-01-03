using BLL.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list booking")]
        public async Task<IActionResult> GetAllBooking([FromQuery] QueryParameters queryParameters)
        {
            var task = await _bookingService.GetListAsync(queryParameters);
            return Ok(task);
        }

        [HttpGet("{bookingId}")]
        [SwaggerOperation(Summary = "[Operator] Get booking detail")]
        public async Task<IActionResult> GetBookingDetail([FromRoute] int bookingId)
        {
            var task = await _bookingService.GetDetailAsync(bookingId);
            return Ok(task);
        }
    }
}
