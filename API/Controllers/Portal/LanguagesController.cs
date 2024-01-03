using BLL.DTOs.Category;
using BLL.DTOs.Language;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Authorize(Roles = "1,2")]
    [Route("api/portal/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Admin] Get List Language")]
        public async Task<IActionResult> GetListLanguage([FromQuery]QueryParameters queryParameters, [FromQuery]bool includeDeleted = false)
        {
            var response = await _languageService.GetListLanguage(queryParameters, includeDeleted);
            return Ok(response);
        }

        [HttpGet("{languageId}")]
        [SwaggerOperation(Summary = "[Admin] Get Language Detail")]
        public async Task<IActionResult> GetListLanguage([FromRoute] int languageId)
        {
            var response = await _languageService.GetLanguageDetail(languageId);
            return Ok(response);
        }

        [HttpPut("changestatus/{languageId}")]
        [SwaggerOperation(Summary = "[Admin] Change status language")]
        public async Task<IActionResult> ChangeStatusCategory([FromRoute]int languageId, [FromQuery]int status)
        {
            var response = await _languageService.ChangeStatusLanguage(languageId,status);
            if (!response)
            {
                throw new BadRequestException("Change Status false!");
            }

            return Ok("Change Status Successfully!");
        }

        [HttpPut("{languageId}")]
        [SwaggerOperation(Summary = "[Admin] Update Language")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int languageId, [FromBody] AddLanguageDto languageUpdate)
        {
            var response = _languageService.UpdateLanguageAsync(languageUpdate, languageId);
            return Ok(await response);
        }
    }
}
