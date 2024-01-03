using BLL.DTOs.Account;
using BLL.DTOs.Staff;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Models;
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
    public class UsersController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UsersController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Admin] Get all list visitor")]
        public async Task<IActionResult> GetAllUser([FromQuery] QueryParameters queryParameters)
        {
            var task = _accountService.GetListVisitor(queryParameters);
            return Ok(await task);
        }

        [HttpGet("{visitorId}")]
        [SwaggerOperation(Summary = "[Admin] Get visitor detail by id")]
        public async Task<IActionResult> GetVisitorDetail([FromRoute] int visitorId)
        {
            var task = await _accountService.GetVisitorDetail(visitorId);
            return Ok(task);
        }

        [HttpPut("{Id}")]
        [SwaggerOperation(Summary = "[Admin] Change status visitor and staff by id")]
        public async Task<IActionResult> DeactiveVisitor([FromRoute] int Id)
        {
            var task = await _accountService.DeactiveVisitorById(Id);
            if (!task)
            {
                throw new BadRequestException("Id not found!");
            }
            return Ok(new StatusResposne("Change Status Successfully!"));
        }

        #region operator
        [HttpGet("operator")]
        [SwaggerOperation(Summary = "[Admin] Get all list operator and admin")]
        public async Task<IActionResult> GetAllOperator([FromQuery] QueryParameters queryParameters)
        {
            var task = _accountService.GetListStaff(queryParameters);
            return Ok(await task);
        }

        [HttpGet("operator/{staffId}")]
        [SwaggerOperation(Summary = "[Admin] Get staff detail by id")]
        public async Task<IActionResult> GetStaffDetail([FromRoute] int staffId)
        {
            var task = await _accountService.GetStaffDetail(staffId);
            return Ok(task);
        }

        [HttpPost("operator")]
        [SwaggerOperation(Summary = "[Admin] create account!")]
        public async Task<IActionResult> CreateOperatorForAdmin([FromBody] CreateStaffDto staffDto)
        {
            var task = await _accountService.CreateNewStaff(staffDto);
            if (!task)
            {
                throw new BadRequestException("Create staff false!");
            }
            return Ok("Create Successfully!");
        }

        [HttpPut("staff/{staffId}")]
        [SwaggerOperation(Summary = "[Admin] update staff information")]
        public async Task<IActionResult> UpdateOpeationProfile([FromBody] StaffUpdateDto UpdateDto,[FromRoute]int staffId)
        {
            var task = _accountService.UpdateStaffAccount(staffId, UpdateDto);
            if (await task)
            {
                return Ok("Update Profile Successfully!");
            }
            else
            {
                throw new BadRequestException("Update Profile False!");
            }
        }
        #endregion
    }
}
