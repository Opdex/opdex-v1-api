using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSwapQuote(SwapQuoteRequestModel request, CancellationToken cancellationToken)
        {
            var query = new GetLiquidityPoolSwapQuoteQuery(request.TokenIn, request.TokenOut, request.TokenInAmount, 
                request.TokenOutAmount, request.TokenInPool, request.TokenOutPool, request.Market);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        
        [HttpPost("add-liquidity")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAddLiquidityQuote(AddLiquidityQuoteRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiquidityPoolAddLiquidityQuoteQuery(request.AmountCrsIn, request.AmountSrcIn, 
                request.Pool, request.Market), cancellationToken);
            
            return Ok(result);
        }
        
        // Todo: Implement - returns the CRS/SRC/USD amount of removing any number of liquidity pool tokens
        // [HttpPost("remove-liquidity")]
        // public Task<IActionResult> CreateRemoveLiquidityQuote(RemoveLiquidityQuoteRequestModel request, CancellationToken cancellationToken)
        // {
        //     throw new NotImplementedException();
        // }
    }
}