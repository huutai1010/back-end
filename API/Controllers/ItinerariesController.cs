using System.Security.Claims;
using BLL.Exceptions;
using BLL.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItinerariesController : ControllerBase
    {
        private readonly IItineraryService _itineraryService;
        public ItinerariesController(IItineraryService itineraryService)
        {
            _itineraryService = itineraryService;

        }
        [HttpGet("{itineraryId}")]
        [SwaggerOperation(Summary = "[Visitor] Get itinerary details", Description = "Get itinerary details")]
        public async Task<IActionResult> GetItineraryById([FromRoute] int itineraryId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");

            var response = await _itineraryService.GetItineraryPlaceByLanguageId(itineraryId, languageCode);
            return Ok(response);
        }
        [HttpGet("top")]
        [SwaggerOperation(Summary = "[Visitor] Get top Itineraries by rating (default is 10)")]
        public async Task<IActionResult> GetTopItinerariesAsync(int? top)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");

            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }

            var result = await _itineraryService.GetTopItinerariesAsync(languageCode, top ?? 10);
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "[Visitor] Get list Itinerary")]
        public async Task<IActionResult> GetItinerariesAsync([FromQuery] QueryParameters queryParameters)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");

            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }

            var result = await _itineraryService.GetItinerariesAsync(languageCode, queryParameters);
            return Ok(result);
        }
        [HttpGet("feedbacks/{itineraryId}")]
        [SwaggerOperation(Summary = "[Visitor] Get itinerary details to feedback", Description = "Get itinerary details to feedback")]
        public async Task<IActionResult> GetItineraryDetail([FromRoute] int itineraryId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");

            var response = await _itineraryService.GetDetailByIdAsync(itineraryId, languageCode);
            return Ok(response);
        }
    }
}