using BLL.DTOs.Tour;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using Common.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class ItinerariesController : ControllerBase
    {
        private readonly IItineraryService _itineraryService;
        public ItinerariesController(IItineraryService tourService)
        {
            _itineraryService = tourService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list itinerary")]
        public async Task<IActionResult> GetItinerariesAsync([FromQuery]QueryParameters queryParameters)
        {
            var response = _itineraryService.GetListAsync(queryParameters);
            return Ok(await response);
        }

        [HttpGet("{itineraryId}")]
        [SwaggerOperation(Summary = "[Operator] Get itinerary details")]
        public async Task<IActionResult> GetItineraryPlaceAsync([FromRoute] int itineraryId)
        {
            var response = _itineraryService.GetDetailAsync(itineraryId);
            return Ok(await response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "[Operator] Create new itinerary")]
        public async Task<IActionResult> CreateItineraryAsync([FromBody] CreateItineraryDto itineraryDto)
        {
            var createId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (createId == null)
            {
                throw new ForbiddenException();
            }

            itineraryDto.CreateById = Int32.Parse(createId);

            var response = _itineraryService.CreateAsync(itineraryDto);
            return Ok(await response);
        }

        [HttpPut("{itineraryId}")]
        [SwaggerOperation(Summary = "[Operator] Update itinerary")]
        public async Task<IActionResult> UpdatItineraryAsync([FromBody] UpdateItineraryDto itineraryDto, [FromRoute] int itineraryId)
        {
            var response = await _itineraryService.UpdateAsync(itineraryDto, itineraryId);
            return Ok(response);
        }

        [HttpPut("changestatus/{itineraryId}")]
        [SwaggerOperation(Summary = "[Operator] Change status itinerary")]
        public async Task<IActionResult> ChangeStatusAsync([FromRoute] int itineraryId)
        {
            var response = await _itineraryService.ChangeStatusAsync(itineraryId);
            if (response)
            {
                return Ok("Change status Successfully!");
            }
            else
            {
                throw new BadRequestException("Change False!");
            }
        }


    }
}
