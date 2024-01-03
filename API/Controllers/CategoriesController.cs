using BLL.Exceptions;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [SwaggerOperation(Summary = "[Visitor] Get categories by language", Description = "Get categories by language")]
        [HttpGet]
        public async Task<IActionResult> GetCategories(string languageCode)
        {
            var response = await _categoryService.GetCategories(languageCode);
            return Ok(response);
        }
    }
}
