using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.WebApi.Models.Requests.Quotes;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("quote")]
    public class QuoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public QuoteController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("swap")]
        public Task<IActionResult> CreateSwapQuote(SwapQuoteRequestModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("add-liquidity")]
        public Task<IActionResult> CreateAddLiquidityQuote(AddLiquidityQuoteRequestModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("remove-liquidity")]
        public Task<IActionResult> CreateRemoveLiquidityQuote(RemoveLiquidityQuoteRequestModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}