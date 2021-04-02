using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet("Pool/{PoolAddress}")]
        public Task<IActionResult> GetTransactionsForPool(string PoolAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("token/{tokenAddress}")]
        public Task<IActionResult> GetTransactionsForToken(string tokenAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}