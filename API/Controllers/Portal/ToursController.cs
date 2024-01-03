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
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;
        public ToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list tour")]
        public async Task<IActionResult> GetToursAsync([FromQuery]QueryParameters queryParameters)
        {
            var response = _tourService.GetListAsync(queryParameters);
            return Ok(await response);
        }

        [HttpGet("{tourId}")]
        [SwaggerOperation(Summary = "[Operator] Get tour detail")]
        public async Task<IActionResult> GetTourDetailAsync([FromRoute] int tourId)
        {
            var response = _tourService.GetDetailAsync(tourId);
            return Ok(await response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "[Operator] Create new tour")]
        public async Task<IActionResult> CreateTourAsync([FromBody] CreateTourDto tourDto)
        {
            var createId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (createId == null)
            {
                throw new ForbiddenException();
            }

            tourDto.CreateById = Int32.Parse(createId);

            var response = _tourService.CreateAsync(tourDto);
            return Ok(await response);
        }

        [HttpPut("{tourId}")]
        [SwaggerOperation(Summary = "[Operator] Update tour")]
        public async Task<IActionResult> UpdateTourAsync([FromBody] UpdateTourDto tourDto, [FromRoute] int tourId)
        {
            var response = await _tourService.UpdateAsync(tourDto, tourId);
            return Ok(response);
        }

        [HttpPut("changestatus/{tourId}")]
        [SwaggerOperation(Summary = "[Operator] Change status tour")]
        public async Task<IActionResult> ChangeStatusAsync([FromRoute] int tourId)
        {
            var response = await _tourService.ChangeStatusAsync(tourId);
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
