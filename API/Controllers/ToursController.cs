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
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;
        public ToursController(ITourService tourService)
        {
            _tourService = tourService;

        }
        [HttpGet("{tourId}")]
        [SwaggerOperation(Summary = "[Visitor] Get tour details", Description = "Get tour details")]
        public async Task<IActionResult> GetTourById([FromRoute] int tourId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");

            var response = await _tourService.GetTourDetailByLanguageId(tourId, languageCode);
            return Ok(response);
        }
        [HttpGet("top")]
        [SwaggerOperation(Summary = "[Visitor] Get top tours by rating (default is 10)")]
        public async Task<IActionResult> GetTopToursAsync(int? top)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");

            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }

            var result = await _tourService.GetTopToursAsync(languageCode, top ?? 10);
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "[Visitor] Get list tour")]
        public async Task<IActionResult> GetToursAsync([FromQuery] QueryParameters queryParameters)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");

            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }

            var result = await _tourService.GetToursAsync(languageCode, queryParameters);
            return Ok(result);
        }
        [HttpGet("feedbacks/{tourId}")]
        [SwaggerOperation(Summary = "[Visitor] Get tour details to feedback", Description = "Get tour details to feedback")]
        public async Task<IActionResult> GetTourDetail([FromRoute] int tourId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");

            var response = await _tourService.GetDetailByIdAsync(tourId, languageCode);
            return Ok(response);
        }
    }
}