using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("quote")]
    public class QuoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public QuoteController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("swap")]
        public Task<IActionResult> CreateSwapQuote(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("add-liquidity")]
        public Task<IActionResult> CreateAddLiquidityQuote(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("remove-liquidity")]
        public Task<IActionResult> CreateRemoveLiquidityQuote(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}