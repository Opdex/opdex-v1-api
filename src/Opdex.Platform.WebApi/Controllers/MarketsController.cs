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

    /// <summary>Create Standard Market Quote</summary>
    /// <remarks>Quote a transaction to create a standard market.</remarks>
    /// <param name="request">Information about the standard market.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("standard")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStandardMarketQuote([FromBody] CreateStandardMarketQuoteRequest request,
                                                                                             CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCreateStandardMarketTransactionQuoteCommand(_context.Wallet,
                                                                                                  request.MarketOwner,
                                                                                                  request.TransactionFee,
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
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStakingMarketQuote([FromBody] CreateStakingMarketQuoteRequest request,
                                                                                            CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCreateStakingMarketTransactionQuoteCommand(_context.Wallet, request.StakingToken), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Get Market</summary>
    /// <remarks>Retrieves a market.</remarks>
    /// <param name="address">The market address to retrieve.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Market</returns>
    [HttpGet("{address}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketDetails([FromRoute] Address address, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketByAddressQuery(address), cancellationToken);

        var response = _mapper.Map<MarketResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Get Market History</summary>
    /// <remarks>Retrieves the history of a market.</remarks>
    /// <param name="address">The market address to retrieve history of.</param>
    /// <param name="filters">Snapshot filters.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Market history with pagination.</returns>
    [HttpGet("{address}/history")]
    [ProducesResponseType(typeof(MarketSnapshotsResponseModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<MarketSnapshotsResponseModel>> GetMarketHistory([FromRoute] Address address,
                                                                                   [FromQuery] MarketSnapshotFilterParameters filters,
                                                                                   CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMarketSnapshotsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<MarketSnapshotsResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Set Ownership Quote</summary>
    /// <remarks>Quote a transaction to set the owner of a standard market, pending a transaction to redeem ownership.</remarks>
    /// <param name="address">The address of the standard market.</param>
    /// <param name="request">Information about the new owner.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{address}/standard/set-ownership")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SetOwnershipQuote([FromRoute] Address address,
                                                                                     [FromBody] SetMarketOwnerQuoteRequest request,
                                                                                     CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSetStandardMarketOwnershipTransactionQuoteCommand(address, _context.Wallet, request.Owner),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Claim Ownership Quote</summary>
    /// <remarks>Quote a transaction to claim ownership of a standard market.</remarks>
    /// <param name="address">The address of the standard market.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{address}/standard/claim-ownership")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> ClaimOwnershipQuote([FromRoute] Address address,
                                                                                       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(address, _context.Wallet),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Set Market Permissions Quote</summary>
    /// <remarks>Quote a transaction to set permissions within a standard market.</remarks>
    /// <param name="address">The address of the standard market.</param>
    /// <param name="walletAddress">The address to assign permissions.</param>
    /// <param name="request">Information about the permissions.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{address}/standard/permissions/{walletAddress}")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SetPermissionsQuote([FromRoute] Address address,
                                                                                       [FromRoute] Address walletAddress,
                                                                                       [FromBody] SetMarketPermissionsQuoteRequest request,
                                                                                       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSetStandardMarketPermissionsTransactionQuoteCommand(address,
                                                                                                          _context.Wallet,
                                                                                                          walletAddress,
                                                                                                          request.Permission,
                                                                                                          request.Authorize), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Get Market Permissions</summary>
    /// <remarks>Retrieves the permissions for an address within a standard market.</remarks>
    /// <param name="marketAddress">The address of the standard market.</param>
    /// <param name="walletAddress">The address for which to retrieve permissions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all market permissions for the wallet address.</returns>
    /// <response code="404">The market could not be found or is not a standard market.</response>
    [HttpGet("{marketAddress}/standard/permissions/{walletAddress}")]
    [ProducesResponseType(typeof(IEnumerable<MarketPermissionType>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MarketPermissionType>>> GetPermissions([FromRoute] Address marketAddress, [FromRoute] Address walletAddress, CancellationToken cancellationToken)
    {
        var permissions = await _mediator.Send(new GetMarketPermissionsForAddressQuery(marketAddress, walletAddress), cancellationToken);
        return Ok(permissions);
    }

    /// <summary>Collect Fees Quote</summary>
    /// <param name="address">The address of the standard market.</param>
    /// <param name="request">Information about the fees to collect.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{address}/standard/collect-fees")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CollectFeesQuote([FromRoute] Address address,
                                                                                    [FromBody] CollectMarketFeesQuoteRequest request,
                                                                                    CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCollectStandardMarketFeesTransactionQuoteCommand(address, _context.Wallet, request.Token, request.Amount),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }
}