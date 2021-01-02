using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("wallet-transactions")]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public WalletTransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [HttpGet]
        public Task<IActionResult> GetAllMyTransactions(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("pair/{pairAddress}")]
        public Task<IActionResult> GetMyTransactionsForPair(string pairAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("token/{tokenAddress}")]
        public Task<IActionResult> GetMyTransactionsForToken(string tokenAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}