using BLL.Interfaces;
using Common.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService= feedbackService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list feedback")]
        public async Task<IActionResult> GetAllListFeedback([FromQuery] QueryParameters queryParameters)
        {
            var response = await _feedbackService.GetFeedbackViewList(queryParameters);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "[Operator] Get list feedback base on itinerary or place id")]
        public async Task<IActionResult> GetAllListFeedbackById([FromRoute]int id ,[FromQuery] QueryParameters queryParameters, [FromQuery] bool isPlace = false)
        {
            var response = await _feedbackService.GetFeedbackDto(queryParameters, id, isPlace);
            return Ok(response);    
        }

        [HttpGet("detail/{feedbackId}")]
        [SwaggerOperation(Summary = "[Operator] get feedback detail by id")]
        public async Task<IActionResult> GetFeedbackById(int feedbackId)
        {
            var response = await _feedbackService.GetFeedbackDetailAsync(feedbackId);
            return Ok(response);
        }
    }
}
