using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public Task<IActionResult> GetAllTransactions(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("pool/{poolAddress}")]
        public async Task<IActionResult> GetTransactionsForPool(string poolAddress, CancellationToken cancellationToken)
        {
            var query = new GetTransactionsByPoolWithFilterQuery(poolAddress, new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10});

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }
        
        [HttpGet("token/{tokenAddress}")]
        public Task<IActionResult> GetTransactionsForToken(string tokenAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}