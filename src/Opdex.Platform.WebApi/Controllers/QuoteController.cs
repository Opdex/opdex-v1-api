using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Quotes;

namespace Opdex.Platform.WebApi.Controllers
{
    /// <summary>
    /// Find quoted transaction outputs for swaps and liquidity providing.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("quote")]
    public class QuoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public QuoteController(IMediator mediator, IMapper mapper, IApplicationContext context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>Swap Amount</summary>
        /// <remarks>
        /// Gets a quote for the amount of tokens returned from any given token swap.
        /// </remarks>
        /// <remarks>
        /// Supports CRS-SRC transactions and SRC-SRC transactions. Given an amount of a token
        /// get the returned amount of another token.
        /// </remarks>
        /// <param name="request">The swap request model for which tokens to swap for and how much to swap.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The amount of tokens to swap for.</returns>
        [HttpPost("swap")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSwapQuote(SwapQuoteRequestModel request, CancellationToken cancellationToken)
        {
            var query = new GetLiquidityPoolSwapQuoteQuery(request.TokenIn, request.TokenOut, request.TokenInAmount, request.TokenOutAmount, _context.Market);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>Add Liquidity Amount</summary>
        /// <remarks>
        /// Gets a quote for how many tokens are required to be input given the other token in a pool's desired input amount.
        /// </remarks>
        /// <param name="request">Request model detailing how many of which tokens are desired to be deposited.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The number of tokens to match the desired input amount of tokens.</returns>
        [HttpPost("add-liquidity")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAddLiquidityQuote(AddLiquidityQuoteRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiquidityPoolAddLiquidityQuoteQuery(request.AmountIn, request.TokenIn,
                request.Pool, _context.Market), cancellationToken);

            return Ok(result);
        }
    }
}
