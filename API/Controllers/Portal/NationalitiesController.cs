using BLL.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Authorize(Roles = "1,2")]
    [Route("api/portal/[controller]")]
    [ApiController]
    public class NationalitiesController : ControllerBase
    {
        private readonly INationalityService _nationalityService;

        public NationalitiesController(INationalityService nationalityService)
        {
            _nationalityService = nationalityService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Admin] Get List Language Code")]
        public async Task<IActionResult> GetListLanguageCode()
        {
            var response = await _nationalityService.GetNationalitiesForAdmin();
            return Ok(response);
        }
    }
}
