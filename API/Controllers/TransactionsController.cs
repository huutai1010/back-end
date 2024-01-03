using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IBookingService _bookingService;

        public TransactionsController(ITransactionService transactionService, IBookingService bookingService)
        {
            _transactionService = transactionService;
            _bookingService = bookingService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Visitor] Get list transaction")]
        public async Task<IActionResult> GetListTransaction([FromQuery] QueryParameters queryParameters)
        {

            bool isValid = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int accountId);
            if (!isValid)
            {
                throw new ForbiddenException();
            }
            var task = await _transactionService.GetListTransaction(queryParameters,accountId);
            return Ok(task);
        }


        [HttpGet("{transactionId}")]
        [SwaggerOperation(Summary = "[Visitor] Get transaction detail")]
        public async Task<IActionResult> GetTransactionDetail([FromRoute] int transactionId)
        {
            var task = await _transactionService.GetTransactionDetail(transactionId);
            return Ok(task);
        }

        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmBooking([FromQuery(Name = "token")]string paymentId, [FromQuery(Name = "payerid")] string payerAccountId)
        {
            var result= await _bookingService.ConfirmBooking(paymentId, payerAccountId);
            return Ok(result);
        }
    }
}
