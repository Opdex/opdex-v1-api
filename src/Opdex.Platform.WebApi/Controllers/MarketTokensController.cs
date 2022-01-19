using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("markets/{market}/tokens")]
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

    /// <summary>Get Market Tokens</summary>
    /// <remarks>Search and filter tokens in a market with pagination.</remarks>
    /// <param name="market">The market contract address to search within.</param>
    /// <param name="filters">Token search filters.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="MarketTokensResponseModel"/> results response with pagination.</returns>
    [HttpGet]
    public async Task<ActionResult<MarketTokensResponseModel>> GetMarketTokens([FromRoute] Address market,
                                                                               [FromQuery] TokenFilterParameters filters,
                                                                               CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketTokensWithFilterQuery(market, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<MarketTokensResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Get Market Token</summary>
    /// <remarks>Returns the market token that matches the provided address.</remarks>
    /// <param name="market">The address of the market smart contract.</param>
    /// <param name="token">The token's smart contract address.</param>
    /// <param name="cancellationToken">cancellation token.</param>
    /// <returns>A market token response.</returns>
    [HttpGet("{token}")]
    public async Task<ActionResult<MarketTokenResponseModel>> GetMarketToken([FromRoute] Address market, [FromRoute] Address token,
                                                                             CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketTokenByMarketAndTokenAddressQuery(market, token), cancellationToken);

        var response = _mapper.Map<MarketTokenResponseModel>(result);

        return Ok(response);
    }

    /// <summary>
    /// Get Market Token History
    /// </summary>
    /// <param name="market">The address of the market contract.</param>
    /// <param name="token">The address of the token contract.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged market snapshot data for the given token.</returns>
    [HttpGet("{token}/history")]
    public async Task<ActionResult<MarketTokenSnapshotsResponseModel>> GetMarketTokenHistory([FromRoute] Address market,
                                                                                             [FromRoute] Address token,
                                                                                             [FromQuery] SnapshotFilterParameters filters,
                                                                                             CancellationToken cancellationToken)
    {
        var marketTokenSnapshotsDto = await _mediator.Send(new GetMarketTokenSnapshotsWithFilterQuery(market, token,
                                                                                                      filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<MarketTokenSnapshotsResponseModel>(marketTokenSnapshotsDto);

        return Ok(response);
    }

    /// <summary>Swap Tokens Quote</summary>
    /// <remarks>Quotes token swap transactions.</remarks>
    /// <param name="market">The address of the market where the token is being sold..</param>
    /// <param name="token">The address of the token being sold, may require allowance.</param>
    /// <param name="request">The token swap request object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A token swap transaction quote.</returns>
    [HttpPost("{token}/swap")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> Swap([FromRoute] Address market, [FromRoute] Address token,
                                                                        [FromBody] SwapRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSwapTransactionQuoteCommand(token, _context.Wallet, request.TokenOut, request.TokenInAmount,
                                                                                  request.TokenOutAmount, request.TokenInMaximumAmount,
                                                                                  request.TokenOutMinimumAmount, request.TokenInExactAmount,
                                                                                  request.Recipient, market, request.Deadline), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Swap Amount In Quote</summary>
    /// <remarks>Retrieve the amount of tokens to be input given an expected output amount of tokens.</remarks>
    /// <param name="market">The address of the market contract.</param>
    /// <param name="token">The input token's contract address.</param>
    /// <param name="request">The amount in swap request containing details of the expected output token and amount.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The amount of tokens to be input.</returns>
    [HttpPost("{token}/swap/amount-in")]
    public async Task<ActionResult<SwapAmountInQuoteResponseModel>> SwapAmountIn([FromRoute] Address market,
                                                                                 [FromRoute] Address token,
                                                                                 [FromBody] SwapAmountInQuoteRequestModel request,
                                                                                 CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSwapAmountInQuery(market, token, request.TokenOut, request.TokenOutAmount), cancellationToken);

        var response = new SwapAmountInQuoteResponseModel { AmountIn = result };

        return Ok(response);
    }

    /// <summary>Swap Amount Out Quote</summary>
    /// <remarks>Retrieve the amount of output tokens given an expected input amount of tokens.</remarks>
    /// <param name="market">The address of the market contract.</param>
    /// <param name="token">The output token's contract address.</param>
    /// <param name="request">The amount out swap request containing details of the expected input token and amount.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The amount of tokens to be output.</returns>
    [HttpPost("{token}/swap/amount-out")]
    public async Task<ActionResult<SwapAmountOutQuoteResponseModel>> SwapAmountOut([FromRoute] Address market,
                                                                                   [FromRoute] Address token,
                                                                                   [FromBody] SwapAmountOutQuoteRequestModel request,
                                                                                   CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSwapAmountOutQuery(market, request.TokenIn, request.TokenInAmount, token), cancellationToken);

        var response = new SwapAmountOutQuoteResponseModel { AmountOut = result };

        return Ok(response);
    }
}