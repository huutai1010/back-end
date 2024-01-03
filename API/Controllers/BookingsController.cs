using BLL.DTOs.Booking;
using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Journey;
using BLL.DTOs.Place.CelebratedPlace;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IJourneyService _journeyService;
        private readonly IPlaceService _placeService;
        public BookingsController(IBookingService bookingService, IJourneyService journeyService, IPlaceService placeService)
        {
            _bookingService = bookingService;
            _journeyService = journeyService;
            _placeService = placeService;
        }
        [HttpGet("celebrated/{journeyId}")]
        [SwaggerOperation(Summary = "[Visitor] Get celebrated place by their journey.")]
        public async Task<ActionResult<List<CelebratedPlaceDto>>> GetCelebratedPlace([FromRoute] int journeyId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var response = _bookingService.GetCelebratedPlace(journeyId, languageCode);
            return Ok(await response);
        }
        [HttpPost("celebrated/{bookingPlaceId}")]
        [SwaggerOperation(Summary = "[Visitor] Create celebrated places.")]
        public async Task<IActionResult> PostCelebratedPlace([FromRoute] int bookingPlaceId, [FromForm]List<IFormFile> imageFiles)
        {
            await _bookingService.PostCelebratedPlace(bookingPlaceId, imageFiles);
            return Ok();
        }

        [HttpGet("/api/history/bookings")]
        [SwaggerOperation(Summary = "[Visitor] Get list History booking.", Description = "Status booking ToPay = 0, Active = 1, Ongoing = 2, Pause = 3, Completed = 4")]
        public async Task<IActionResult> GetHisitoryBooking([FromQuery] QueryParameters queryParameters)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _bookingService.GetHistoryBooking(queryParameters, accountId, languageCode);
            return Ok(result);
        }
        [HttpGet("/api/history/bookings/information/{bookingId}")]
        [SwaggerOperation(Summary = "[Visitor] Get booking detail information")]
        public async Task<IActionResult> GetInformationBookingDetail([FromRoute] int bookingId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _bookingService.GetInformationBookingDetail(bookingId, languageCode);
            return Ok(result);
        }

        [HttpGet("/api/checkin/{bookingPlaceId}")]
        [SwaggerOperation(Summary = "[Visitor] Checkin place.", Description = "  Future = 0, Ongoing = 1 , Completed = 2")]
        public async Task<IActionResult> CheckInPlace([FromRoute] int bookingPlaceId, [FromQuery] bool isFinish)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _bookingService.CheckInPlace(bookingPlaceId, isFinish);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] PostBookingDto bookingDto)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }

            var links = await _bookingService.CreateBookingAsync(accountId, bookingDto);
            return Ok(links);
        }

        [HttpGet("/api/history/bookings/journeys/status/{status}")]
        [SwaggerOperation(Summary = "[Visitor] Get all journey in warehouse by status", Description = " Journey status Future = 0, Ongoing = 1 , Completed = 2,")]
        public async Task<IActionResult> GetListJourney([FromRoute] int status)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _journeyService.GetListJourney(status, accountId,languageCode);
            return Ok(result);
        }

        [HttpGet("/api/history/bookings/place")]
        [SwaggerOperation(Summary = "[Visitor] GET all place in warehouse by status", Description = "Place not going IsJourney = false,place has gone IsJourney = true ")]
        public async Task<IActionResult> GetHistoryPlace([FromQuery] bool isJourney)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _bookingService.GetHistoryBookingPlace(isJourney, languageCode, accountId);
            return Ok(result);
        }

        [HttpPost("journey")]
        [SwaggerOperation(Summary = "[Visitor] Create new journey")]
        public async Task<IActionResult> CreateJourney([FromBody] PostJourneyDto journeyDto)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _journeyService.CreateJourney(journeyDto, languageCode);
            return Ok(result);
        }

        [HttpGet("/api/history/bookings/journey/{journeyId}")]
        [SwaggerOperation(Summary = "[Visitor] Get journey detail")]
        public async Task<IActionResult> GetJourneyDetail([FromRoute] int journeyId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _journeyService.GetJourneyDetail(journeyId, languageCode);
            return Ok(result);
        }

        [HttpPut("/api/history/bookings/journey/{journeyId}/{status}")]
        [SwaggerOperation(Summary = "[Visitor] Update journey status")]
        public async Task<IActionResult> PutJourneyStatus([FromRoute] int journeyId,[FromRoute] int status)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var result = await _journeyService.PutJourneyStatus(journeyId, status);
            return Ok(result);
        }

        [HttpGet("/api/history/bookings/place/{placeId}")]
        [SwaggerOperation(Summary = "[Visitor] Get data place at voice screen", Description = ".")]
        public async Task<IActionResult> GetVoiceScreenData([FromRoute] int placeId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");
            var response = await _placeService.GetVoiceScreenData(placeId,languageCode);
            return Ok(response);
        }

        [HttpPut("{bookingId}")]
        [SwaggerOperation(Summary = "[Visitor] Cancel booking ONLY when status booking is ToPAY")]
        public async Task<IActionResult> CancelBooking([FromRoute] int bookingId)
        {
            await _bookingService.CancelBooking(bookingId);
            return Ok();
        }

        [HttpGet("cancel/batch")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelBatchBooking()
        {
            await _bookingService.CancelBatchBooking();
            return Ok();
        }
    }
}
