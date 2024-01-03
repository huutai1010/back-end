using BLL.Exceptions;
using BLL.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Portal
{
    [Route("api/portal/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "[Operator] Get list transaction")]
        public async Task<IActionResult> GetListTransaction([FromQuery]QueryParameters queryParameters)
        {
            var response = await _transactionService.GetListAsync(queryParameters);
            return Ok(response);
        }

        [HttpGet("{transactionid}")]
        [SwaggerOperation(Summary = "[Operator] Get transaction detail")]
        public async Task<IActionResult> GetTransactionDetail([FromRoute]int transactionid)
        { 
            var response = await _transactionService.GetTransactionDetailAsync(transactionid);
            if(response is null)
            {
                throw new NotFoundException($"Transaction id: {transactionid} not found!");
            }
            return Ok(response);
        }
    }
}
