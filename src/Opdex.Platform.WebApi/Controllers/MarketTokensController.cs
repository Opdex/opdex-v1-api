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
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
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

        /// <summary> Get Market Tokens</summary>
        /// <remarks>Search and filter tokens in a market.</remarks>
        /// <param name="marketAddress"></param>
        /// <param name="filters"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="MarketTokensResponseModel"/> results response with pagination.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(MarketTokensResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MarketTokensResponseModel>> GetMarketTokens([FromRoute] Address marketAddress,
                                                                                   [FromQuery] TokenFilterParameters filters,
                                                                                   CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketTokensWithFilterQuery(marketAddress, filters.BuildCursor()), cancellationToken);

            var response = _mapper.Map<MarketTokensResponseModel>(result);

            return Ok(response);
        }

        /// <summary>Get Market Token</summary>
        /// <remarks>Returns the market token that matches the provided address.</remarks>
        /// <param name="marketAddress">The address of the market smart contract.</param>
        /// <param name="tokenAddress">The token's smart contract address.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>A market token response.</returns>
        [HttpGet("{tokenAddress}")]
        [ProducesResponseType(typeof(MarketTokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MarketTokenResponseModel>> GetMarketToken([FromRoute] Address marketAddress, [FromRoute] Address tokenAddress,
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
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> Swap([FromRoute] Address marketAddress, [FromRoute] Address tokenAddress,
                                                                            [FromBody] SwapRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSwapTransactionQuoteCommand(tokenAddress, _context.Wallet, request.TokenOut, request.TokenInAmount,
                                                                                      request.TokenOutAmount, request.TokenInMaximumAmount,
                                                                                      request.TokenOutMinimumAmount, request.TokenInExactAmount,
                                                                                      request.Recipient, marketAddress, request.Deadline), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Swap Amount In Quote</summary>
        /// <remarks>Retrieve the amount of tokens to be input given an expected output amount of tokens.</remarks>
        /// <param name="marketAddress">The address of the market contract.</param>
        /// <param name="tokenIn">The input token's contract address.</param>
        /// <param name="request">The amount in swap request containing details of the expected output token and amount.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The amount of tokens to be input.</returns>
        [HttpPost("{tokenIn}/swap/amount-in")]
        [ProducesResponseType(typeof(SwapAmountInQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwapAmountInQuoteResponseModel>> SwapAmountIn([FromRoute] Address marketAddress,
                                                                                     [FromRoute] Address tokenIn,
                                                                                     [FromBody] SwapAmountInQuoteRequestModel request,
                                                                                     CancellationToken cancellationToken)
        {
            // Todo: Revisit and split out this query's flow to Amount In specifically
            var query = new GetLiquidityPoolSwapQuoteQuery(tokenIn, request.TokenOut, FixedDecimal.Zero, request.TokenOutAmount, marketAddress);

            var result = await _mediator.Send(query, cancellationToken);

            var response = new SwapAmountInQuoteResponseModel { AmountIn = result };

            return Ok(response);
        }

        /// <summary>Swap Amount Out Quote</summary>
        /// <remarks>Retrieve the amount of output tokens given an expected input amount of tokens.</remarks>
        /// <param name="marketAddress">The address of the market contract.</param>
        /// <param name="tokenOut">The output token's contract address.</param>
        /// <param name="request">The amount out swap request containing details of the expected input token and amount.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The amount of tokens to be output.</returns>
        [HttpPost("{tokenOut}/swap/amount-out")]
        [ProducesResponseType(typeof(SwapAmountOutQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwapAmountOutQuoteResponseModel>> SwapAmountOut([FromRoute] Address marketAddress,
                                                                                       [FromRoute] Address tokenOut,
                                                                                       [FromBody] SwapAmountOutQuoteRequestModel request,
                                                                                       CancellationToken cancellationToken)
        {
            // Todo: Revisit and split out this query's flow to Amount Out specifically
            var query = new GetLiquidityPoolSwapQuoteQuery(request.TokenIn, tokenOut, request.TokenInAmount, FixedDecimal.Zero, marketAddress);

            var result = await _mediator.Send(query, cancellationToken);

            var response = new SwapAmountOutQuoteResponseModel { AmountOut = result };

            return Ok(response);
        }
    }
}
