using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Enums;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("markets")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
public class MarketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IApplicationContext _context;

    public MarketsController(IMediator mediator, IMapper mapper, IApplicationContext context)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>Get Markets</summary>
    /// <remarks>Retrieves all of the markets tracked by the API</remarks>
    /// <param name="filters">Query filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Market results</returns>
    [HttpGet]
    public async Task<ActionResult<MarketsResponseModel>> GetMarkets(
        [FromQuery] MarketFilterParameters filters,
        CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetMarketsWithFilterQuery(filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<MarketsResponseModel>(vaults));
    }

    /// <summary>Create Standard Market Quote</summary>
    /// <remarks>Quote a transaction to create a standard market.</remarks>
    /// <param name="request">Information about the standard market.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("standard")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStandardMarketQuote([FromBody] CreateStandardMarketQuoteRequest request,
                                                                                             CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCreateStandardMarketTransactionQuoteCommand(_context.Wallet,
                                                                                                  request.Owner,
                                                                                                  request.TransactionFeePercent,
                                                                                                  request.AuthPoolCreators,
                                                                                                  request.AuthLiquidityProviders,
                                                                                                  request.AuthTraders,
                                                                                                  request.EnableMarketFee), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Create Staking Market Quote</summary>
    /// <param name="request">Information about the staking market.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("staking")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStakingMarketQuote([FromBody] CreateStakingMarketQuoteRequest request,
                                                                                            CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCreateStakingMarketTransactionQuoteCommand(_context.Wallet, request.StakingToken), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Get Market</summary>
    /// <remarks>Retrieves a market.</remarks>
    /// <param name="market">The market address to retrieve.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Market details with its daily summary</returns>
    [HttpGet("{market}")]
    public async Task<ActionResult<MarketResponseModel>> GetMarketDetails([FromRoute] Address market, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketByAddressQuery(market), cancellationToken);

        var response = _mapper.Map<MarketResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Get Market History</summary>
    /// <remarks>Retrieves the history of a market.</remarks>
    /// <param name="market">The market address to retrieve history of.</param>
    /// <param name="filters">Snapshot filters.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Market history with pagination.</returns>
    [HttpGet("{market}/history")]
    public async Task<ActionResult<MarketSnapshotsResponseModel>> GetMarketHistory([FromRoute] Address market,
                                                                                   [FromQuery] MarketSnapshotFilterParameters filters,
                                                                                   CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketSnapshotsWithFilterQuery(market, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<MarketSnapshotsResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Set Ownership Quote</summary>
    /// <remarks>Quote a transaction to set the owner of a standard market, pending a transaction to redeem ownership.</remarks>
    /// <param name="market">The address of the standard market.</param>
    /// <param name="request">Information about the new owner.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{market}/standard/set-ownership")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SetOwnershipQuote([FromRoute] Address market,
                                                                                     [FromBody] SetMarketOwnerQuoteRequest request,
                                                                                     CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSetStandardMarketOwnershipTransactionQuoteCommand(market, _context.Wallet, request.Owner),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Claim Ownership Quote</summary>
    /// <remarks>Quote a transaction to claim ownership of a standard market.</remarks>
    /// <param name="market">The address of the standard market.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{market}/standard/claim-ownership")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> ClaimOwnershipQuote([FromRoute] Address market,
                                                                                       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(market, _context.Wallet),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Set Market Permissions Quote</summary>
    /// <remarks>Quote a transaction to set permissions within a standard market.</remarks>
    /// <param name="market">The address of the standard market.</param>
    /// <param name="address">The address to assign permissions.</param>
    /// <param name="request">Information about the permissions.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{market}/standard/permissions/{address}")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SetPermissionsQuote([FromRoute] Address market,
                                                                                       [FromRoute] Address address,
                                                                                       [FromBody] SetMarketPermissionsQuoteRequest request,
                                                                                       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSetStandardMarketPermissionsTransactionQuoteCommand(market,
                                                                                                          _context.Wallet,
                                                                                                          address,
                                                                                                          request.Permission,
                                                                                                          request.Authorize), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Get Market Permissions</summary>
    /// <remarks>Retrieves the permissions for an address within a standard market.</remarks>
    /// <param name="market">The address of the standard market.</param>
    /// <param name="address">The address for which to retrieve permissions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all market permissions for the wallet address.</returns>
    [HttpGet("{market}/standard/permissions/{address}")]
    public async Task<ActionResult<IEnumerable<MarketPermissionType>>> GetPermissions([FromRoute] Address market, [FromRoute] Address address, CancellationToken cancellationToken)
    {
        var permissions = await _mediator.Send(new GetMarketPermissionsForAddressQuery(market, address), cancellationToken);
        return Ok(permissions);
    }

    /// <summary>Collect Fees Quote</summary>
    /// <param name="market">The address of the standard market.</param>
    /// <param name="request">Information about the fees to collect.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{market}/standard/collect-fees")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CollectFeesQuote([FromRoute] Address market,
                                                                                    [FromBody] CollectMarketFeesQuoteRequest request,
                                                                                    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCollectStandardMarketFeesTransactionQuoteCommand(market, _context.Wallet, request.Token, request.Amount),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }
}
