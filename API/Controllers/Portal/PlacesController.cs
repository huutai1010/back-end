using Aspose.Cells;
using BLL.DTOs.Place;
using BLL.DTOs.Place.PlaceItem;
using BLL.Exceptions;
using BLL.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]

    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly IFeedbackService _feedbackService;
        public PlacesController(IPlaceService placeService, IFeedbackService feedbackService)
        {
            _placeService = placeService;
            _feedbackService = feedbackService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list place")]
        public async Task<IActionResult> GetPlacesAsync([FromQuery] QueryParameters parameters)
        {
            var response = await _placeService.GetListAsync(parameters);
            return Ok(response);
        }

        [HttpGet("{placeId}")]
        [SwaggerOperation(Summary = "[Operator] Get place detail by id")]
        public async Task<IActionResult> GetPlaceById([FromRoute] int placeId)
        {
            var response = await _placeService.GetDetailAsync(placeId);
            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "[Operator] Create new place")]
        public async Task<IActionResult> CreatePlaceAsync([FromBody] CreatePlaceDto placeDto)
        {
            var response = await _placeService.CreateAsync(placeDto);
            return Ok(response);
        }

        [HttpPut("{placeId}")]
        [SwaggerOperation(Summary = "[Operator] Update place")]
        public async Task<IActionResult> UpdatePlaceAsync([FromBody] UpdatePlaceDto placeDto, [FromRoute] int placeId)
        {
            var response = await _placeService.UpdateAsync(placeDto, placeId);
            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                throw new BadRequestException("Update False!");
            }
        }

        [HttpPut("changestatus/{placeId}")]
        [SwaggerOperation(Summary = "[Operator] Change status place")]
        public async Task<IActionResult> ChangeStatusAsync([FromRoute] int placeId, [FromQuery] int status)
        {
            var response = await _placeService.ChangeStatusAsync(placeId, status);
            if (response)
            {
                return Ok("Change Status Place Successfully!");
            }
            else
            {
                throw new BadRequestException("Change False!");
            }
        }

        [HttpPost("importExcel")]
        [SwaggerOperation(Summary = "[Operator] import excel for places")]
        public async Task<IActionResult> ImportExcelPlaceAsync(List<IFormFile> fileList)
        {
            var excelFile = fileList.Where(x => x.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            if(excelFile == null)
            {
                throw new NotFoundException("Excel file is missing!");
            }
            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                if (!excelFile.FileName.EndsWith(".xlsx"))
                {
                    throw new BadRequestException("The file format is not supported!");
                }
                var voiceFiles = fileList.Where(x => x.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)).ToList();
                var images = fileList.Where(x => x.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || x.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();
                var task = await _placeService.ImportExcelForPlace(stream, images, voiceFiles);

                if (task == false)
                {
                    throw new BadRequestException("Import data false!");
                }
            }

            return Ok("Import data successfully!");
        }

        [HttpPut("feedback/{feedbackId}")]
        [SwaggerOperation(Summary = "[Operator] Get feedback for place or itinerary")]
        public async Task<IActionResult> ChangeStatusFeedback([FromRoute] int feedbackId)
        {
            var check = await _feedbackService.ChangeStatusFeedback(feedbackId);
            if(check == false)
            {
                throw new BadRequestException("Feedback not found!");
            }
            return Ok("Change status successfully!");
        }

        [HttpPut("placeItem/{placeItemId}")]
        [SwaggerOperation(Summary = "[Operator] Update place item beacon in place detail")]
        public async Task<IActionResult> UpdatePlaceItemAsync([FromBody] PlaceUpdateItemDto placeItemDto, [FromRoute] int placeItemId)
        {
            var response = await _placeService.UpdatePlaceItemAsync(placeItemDto, placeItemId);
            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                throw new BadRequestException("Update False!");
            }
        }

        [HttpGet("placeItem/{placeItemId}")]
        [SwaggerOperation(Summary = "[Operator] Get place item detail by id")]
        public async Task<IActionResult> GetPlaceItemById([FromRoute] int placeItemId)
        {
            var response = await _placeService.GetPlaceItemAsync(placeItemId);
            return Ok(response);
        }
    }
}
