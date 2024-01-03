using BLL.DTOs.Language;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [SwaggerOperation(Summary = "Get Languages")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<LanguageDto>>> GetLanguages()
        {
            var result = await _languageService.GetLanguages();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Language Details")]
        [AllowAnonymous]
        public async Task<ActionResult<LanguageDto>> GetLanguageById(int id)
        {
            var result = await _languageService.GetLanguageById(id);

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create new language.")]
        public async Task<ActionResult<LanguageDto>> PostLanguages(AddLanguageDto languageDto)
        {
            var result = await _languageService.PostLanguages(languageDto);

            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update a language.")]
        public async Task<ActionResult<LanguageDto>> PutLanguages(int id, LanguageDto languageDto)
        {
            var result = await _languageService.PutLanguage(id, languageDto);
            return Ok(result);
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Delete a language.")]
        public async Task<ActionResult<LanguageDto>> DeleteLanguages(int id)
        {
            var result = await _languageService.DeleteLanguages(id);

            return Ok(result);
        }
    }
}
