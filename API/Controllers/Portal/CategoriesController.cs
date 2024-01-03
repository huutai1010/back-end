using BLL.DTOs.Category;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using Common.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Authorize(Roles = "1,2")]
    [Route("api/portal/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Admin] Get List Category")]
        public async Task<IActionResult> GetListCategory([FromQuery] QueryParameters queryParameters)
        {
            var response = await _categoryService.GetListAsync(queryParameters);
            return Ok(response);
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "[Operator] Get all Category without paging")]
        public async Task<IActionResult> GetAllCategory()
        {
            var reponse = await _categoryService.GetCategoriesWithoutPaging();
            return Ok(reponse);
        }

        [HttpGet("{categoryId}")]
        [SwaggerOperation(Summary = "[Admin] Get Category")]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var response = await _categoryService.GetDetailAsync(categoryId);
            return Ok(response);
        }

        [HttpPut("changestatus/{categoryId}")]
        [SwaggerOperation(Summary = "[Admin] Change status Category")]
        public async Task<IActionResult> ChangeStatusCategory(int categoryId)
        {
            var response = await _categoryService.ChangeStatusAsync(categoryId);
            if (!response)
            {
                throw new BadRequestException("Change Status false!");
            }

            return Ok("Change Status Successfully!");
        }

        [HttpPost]
        [SwaggerOperation(Summary = "[Admin] Create Category")]
        public async Task<IActionResult> CreateCategoryForAdmin([FromBody] CategoryCreateDto categoryCreate)
        {
            var response = await _categoryService.CreateCategoryAsync(categoryCreate);
            if (!response)
            {
                throw new BadRequestException("Create Category false!");
            }
            return Ok("Create Successfully!");
        }

        [HttpPut("{categoryId}")]
        [SwaggerOperation(Summary = "[Admin] Update Category")]
        public async Task<IActionResult> UpdateCategory([FromRoute]int categoryId, [FromBody] CategoryUpdateDto categoryUpdate)
        {
            var response = _categoryService.UpdateCategoryAsync(categoryUpdate,categoryId);
            return Ok(await response);
        }
    }
}
