﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tokens")]
public class TokensController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IApplicationContext _context;

    public TokensController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>Get Tokens</summary>
    /// <remarks>Retrieve tokens known to Opdex with filtering and pagination.</remarks>
    /// <param name="filters">Token search filters.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtered tokens with paging.</returns>
    /// <response code="200">Token results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet]
    public async Task<ActionResult<TokensResponseModel>> GetTokens([FromQuery] TokenFilterParameters filters, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTokensWithFilterQuery(filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<TokensResponseModel>(result);

        return Ok(response);
    }

    /// <summary>
    /// Add Token
    /// </summary>
    /// <remarks>Adds an SRC token to the Opdex indexer that can be tracked and used within markets.</remarks>
    /// <param name="request">Token details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Token details.</returns>
    /// <response code="201">The token was added to indexed tokens.</response>
    /// <response code="303">Token is already indexed. Redirects to `GET /tokens/{address}`.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    public async Task<IActionResult> AddToken([FromBody] AddTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAddTokenCommand(request.TokenAddress), cancellationToken);

        var response = _mapper.Map<TokenResponseModel>(result);

        return Created($"/tokens/{request.TokenAddress}", response);
    }

    /// <summary>Get Token</summary>
    /// <remarks>Returns the token that matches the provided address.</remarks>
    /// <param name="address">Address of the token.</param>
    /// <param name="cancellationToken">cancellation token.</param>
    /// <returns>Token details.</returns>
    /// <response code="200">Token details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Token not found.</response>
    [HttpGet("{address}")]
    public async Task<ActionResult<TokenResponseModel>> GetToken([FromRoute] Address address, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTokenByAddressQuery(address), cancellationToken);

        var response = _mapper.Map<TokenResponseModel>(result);

        return Ok(response);
    }

    ///<summary>Get Token History</summary>
    /// <remarks>Retrieve historical data points for a token tracking open, high, low, and close of USD prices.</remarks>
    /// <param name="address">The address of the token.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged token snapshot data.</returns>
    /// <response code="200">Returned token snapshots.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Token not found.</response>
    [HttpGet("{address}/history")]
    public async Task<ActionResult<TokenSnapshotsResponseModel>> GetTokenHistory([FromRoute] Address address, [FromQuery] SnapshotFilterParameters filters,
                                                                                 CancellationToken cancellationToken)
    {
        var tokenSnapshotDtos = await _mediator.Send(new GetTokenSnapshotsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<TokenSnapshotsResponseModel>(tokenSnapshotDtos);

        return Ok(response);
    }

    /// <summary>Approve Allowance Quote</summary>
    /// <remarks>Quotes a transaction to approve an SRC token allowance for a spender.</remarks>
    /// <param name="address">The token address to approve an allowance of.</param>
    /// <param name="request">The allowance approval request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction quote of an approve allowance transaction.</returns>
    /// <response code="200">Approve allowance quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Token not found.</response>
    [HttpPost("{address}/approve")]
    public async Task<IActionResult> ApproveAllowance([FromRoute] Address address, [FromBody] ApproveAllowanceQuoteRequest request, CancellationToken cancellationToken)
    {
        if (address == Address.Cirrus) throw new InvalidDataException(nameof(address), "Address must be SRC token address.");

        var response = await _mediator.Send(new CreateApproveAllowanceTransactionQuoteCommand(address, _context.Wallet, request.Spender, request.Amount),
                                            cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Distribute Tokens Quote</summary>
    /// <remarks>Quotes a transaction to distribute SRC tokens on eligible token types.</remarks>
    /// <param name="address">The address of the token smart contract.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Token distribute transaction quote.</returns>
    /// <response code="200">Distribute quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Token not found.</response>
    [HttpPost("{address}/distribute")]
    public async Task<IActionResult> Distribute([FromRoute] Address address, CancellationToken cancellationToken)
    {
        if (address == Address.Cirrus) throw new InvalidDataException(nameof(address), "Address must be SRC token address.");

        var response = await _mediator.Send(new CreateDistributeTokensTransactionQuoteCommand(address, _context.Wallet), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }
}
