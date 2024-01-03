using BLL.Exceptions;
using BLL.Interfaces;
using Common.Distances;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }


        [HttpGet("{placeId}")]
        [SwaggerOperation(Summary = "[Visitor] Get place details", Description = "Get place details")]
        public async Task<IActionResult> GetPlaceById([FromRoute] int placeId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var languageCode = User.FindFirstValue("language_code");
            var response = await _placeService.GetPlace(placeId, accountId, languageCode);
            return Ok(response);
        }
        [HttpGet("nearby")]
        [SwaggerOperation(Summary = "[Visitor] Get places around visitor", Description = "Get Place Near Visitor is used to display on the screen 1.2.2 Search Place in the recommended section. Example : ?longitudeVisitor=106.699018&latitudeVisitor=10.779782")]
        public async Task<IActionResult> GetPlaceNearVisitor(double longitudeVisitor, double latitudeVisitor)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            var languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }

            GeoPoint currentLoc = new GeoPoint
            {
                Latitude = latitudeVisitor,
                Longitude = longitudeVisitor
            };
            var response = await _placeService.GetPlaceNearVisitor(currentLoc, languageCode);
            return Ok(response);
        }

        [HttpGet("top")]
        [SwaggerOperation(Summary = "[Visitor] Get top places by rating (default is 10)")]

        public async Task<IActionResult> GetTopPlaces([FromQuery] int? top)
        {

            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var result = await _placeService.GetTopPlacesAsync(languageCode, top ?? 10);
            return Ok(result);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "[Visitor] Search place", Description = "Search Place in search screen")]
        public async Task<IActionResult> SearchPlace([FromQuery] string category)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            int[] categoryId = category.Split(',').Select(x=>int.Parse(x)).ToArray();
            var response = await _placeService.SearchPlaces(categoryId, languageCode);
            return Ok(response);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "[Visitor] Get list places")]
        public async Task<IActionResult> GetPlaces([FromQuery] QueryParameters queryParameters)
        {

            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var result = await _placeService.GetPlaces(queryParameters,languageCode);
            return Ok(result);
        }

        [HttpGet("{placeId}/items")]
        [SwaggerOperation(Summary = "[Visitor] Get list beacon at place")]
        public async Task<IActionResult> GetBeaconsByPlaceId([FromRoute] int placeId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var result = await _placeService.GetBeaconsByPlaceId(placeId, languageCode);
            return Ok(result);
        }


        [HttpGet("placeItems/{placeItemId}")]
        [SwaggerOperation(Summary = "[Visitor] Get beacon name language code")]
        public async Task<IActionResult> GetBeaconData([FromRoute] int placeItemId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            string languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0)
            {
                throw new ForbiddenException();
            }
            var result = await _placeService.GetBeaconData(placeItemId, languageCode);
            return Ok(result);
        }

        [HttpGet("nearby/{placeId}")]
        [SwaggerOperation(Summary = "[Visitor]Find places nearby place", Description = "Get Places Nearby Place")]
        public async Task<IActionResult> GetPlacesNearPlace([FromRoute] int placeId)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            var languageCode = User.FindFirstValue("language_code");
            if (!isValid || accountId <= 0 || string.IsNullOrEmpty(languageCode))
            {
                throw new ForbiddenException();
            }
            var response = await _placeService.GetPlacesNearPlace(placeId, languageCode);
            return Ok(response);
        }
    }
}
