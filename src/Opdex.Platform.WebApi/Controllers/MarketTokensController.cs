using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Quotes;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("market/{marketAddress}/tokens")]
    public class MarketTokensController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public MarketTokensController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        /// <summary>Get Token</summary>
        /// <remarks>Returns the token that matches the provided address.</remarks>
        /// <param name="marketAddress">The address of the market smart contract.</param>
        /// <param name="tokenAddress">The token's smart contract address.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns><see cref="TokenResponseModel"/> of the requested token</returns>
        [HttpGet("{tokenAddress}")]
        [ProducesResponseType(typeof(MarketTokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MarketTokenResponseModel>> MarketToken([FromRoute] Address marketAddress, [FromRoute] Address tokenAddress,
                                                                              CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketTokenByMarketAndTokenAddressQuery(marketAddress, tokenAddress), cancellationToken);

            var response = _mapper.Map<MarketTokenResponseModel>(result);

            return Ok(response);
        }

        /// <summary>Swap Tokens Quote</summary>
        /// <remarks>Quotes token swap transactions.</remarks>
        /// <param name="marketAddress">The address of the market where the token is being sold..</param>
        /// <param name="tokenAddress">The address of the token being sold, may require allowance.</param>
        /// <param name="request">The token swap request object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A token swap transaction quote.</returns>
        [HttpPost("{tokenAddress}/swap")]
        [ProducesResponseType(typeof(ActionResult<TransactionQuoteResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<ProblemDetails>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Swap([FromRoute] Address marketAddress, [FromRoute] Address tokenAddress,
                                              SwapRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSwapTransactionQuoteCommand(tokenAddress, _context.Wallet, request.TokenOut, request.TokenInAmount,
                                                                                      request.TokenOutAmount, request.TokenInMaximumAmount,
                                                                                      request.TokenOutMinimumAmount, request.TokenInExactAmount,
                                                                                      request.Recipient, marketAddress, request.Deadline), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Swap Amount</summary>
        /// <remarks>
        /// Gets a quote for the amount of tokens returned from any given token swap.
        /// </remarks>
        /// <remarks>
        /// Supports CRS-SRC transactions and SRC-SRC transactions. Given an amount of a token
        /// get the returned amount of another token.
        /// </remarks>
        /// <param name="marketAddress"></param>
        /// <param name="tokenAddress"></param>
        /// <param name="request">The swap request model for which tokens to swap for and how much to swap.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The amount of tokens to swap for.</returns>
        [HttpPost("{tokenAddress}/swap/amount")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSwapQuote([FromRoute] Address marketAddress, [FromRoute] Address tokenAddress,
                                                      [FromBody] SwapQuoteRequestModel request, CancellationToken cancellationToken)
        {
            var query = new GetLiquidityPoolSwapQuoteQuery(request.TokenIn, request.TokenOut, request.TokenInAmount, request.TokenOutAmount, marketAddress);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
