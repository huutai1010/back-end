using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NationalitiesController : ControllerBase
    {
        private readonly INationalityService _nationalityService;

        public NationalitiesController(INationalityService nationalityService)
        {
            _nationalityService = nationalityService;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "[Visitor] Get List Nationality")]
        public async Task<IActionResult> GetNationality()
        {
            var response = await _nationalityService.GetNationalities();
            return Ok(response);
        }
    }
}
