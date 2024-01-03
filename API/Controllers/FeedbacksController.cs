
using BLL.DTOs.Feedback;
using BLL.Exceptions;
using BLL.Interfaces;
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
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet("{targetId}")]
        [SwaggerOperation(Summary = "[Visitor] Get Feedbacks", Description = "Get Feedbacks by ItineraryId (isPlace = false) or PlaceID (isPlace = true).", OperationId = "getFeedbacks")]
        public async Task<ActionResult<PagedResult<FeedbackListDto>>> GetFeedbacksAsync([FromQuery] QueryParameters queryParameters, int targetId, bool isPlace)
        {
            var result = await _feedbackService.GetFeedbacksAsync(queryParameters, targetId, isPlace);

            return Ok(result);
        }
        [HttpPost]
        [SwaggerOperation(Summary = "[Visitor] Create a feedback", Description = "Create a feedback for Itinerary (isPlace = false) or Place (isPlace = true)", OperationId = "postFeedbacks")]
        public async Task<ActionResult<AddFeedbackDto>> PostFeedbacks(AddFeedbackDto addFeedbackDto)
        {
            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (!isValid || userId <= 0)
            {
                throw new ForbiddenException();
            }
            await _feedbackService.PostFeedbacks(addFeedbackDto, userId);

            return NoContent();
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Delete Feedbacks", Description = "Delete feedback.", OperationId = "deleteFeedbacks")]

        public async Task<ActionResult<FeedbackListDto>> DeleteFeedbacks(int id)
        {
            var result = await _feedbackService.DeleteFeedback(id);

            return Ok(result);
        }
    }
}
