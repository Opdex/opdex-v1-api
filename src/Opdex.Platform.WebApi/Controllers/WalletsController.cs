using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using Opdex.Platform.WebApi.OpenApi.Wallets;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("wallets")]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public WalletsController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>Get Approved Allowance</summary>
    /// <remarks>Retrieve the allowance of a spender for tokens owned by another wallet.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="token">Address of the token.</param>
    /// <param name="spender">Address for the spender of the allowance.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Approved allowance summary</returns>
    [HttpGet("{address}/allowance/{token}/approved/{spender}")]
    [OpenApiOperationProcessor(typeof(GetAllowanceOperationProcessor))]
    [ProducesResponseType(typeof(ApprovedAllowanceResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovedAllowanceResponseModel>> GetAllowance([FromRoute] Address address,
                                                                                 [FromRoute] Address token,
                                                                                 [FromRoute] Address spender,
                                                                                 CancellationToken cancellationToken)
    {
        if (token == Address.Cirrus) throw new InvalidDataException(nameof(token), "Token must be network address for an SRC token.");
        var allowances = await _mediator.Send(new GetAddressAllowanceQuery(address, spender, token), cancellationToken);
        var response = _mapper.Map<ApprovedAllowanceResponseModel>(allowances);
        return Ok(response);
    }

    /// <summary>Get Balances</summary>
    /// <remarks>Retrieves token balances for an address.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of address balance summaries by token.</returns>
    [HttpGet("{address}/balance")]
    [OpenApiOperationProcessor(typeof(GetAddressBalancesOperationProcessor))]
    [ProducesResponseType(typeof(AddressBalancesResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AddressBalancesResponseModel>> GetAddressBalances([FromRoute] Address address,
                                                                                     [FromQuery] AddressBalanceFilterParameters filters,
                                                                                     CancellationToken cancellationToken)
    {
        var balances = await _mediator.Send(new GetAddressBalancesWithFilterQuery(address, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<AddressBalancesResponseModel>(balances);

        return Ok(response);
    }

    /// <summary>Get Balance</summary>
    /// <remarks>Retrieves the indexed balance of a token for an address.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="token">Address of the token to get the balance of, or CRS.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Address balance summary.</returns>
    [HttpGet("{address}/balance/{token}")]
    [OpenApiOperationProcessor(typeof(GetAddressBalanceByTokenOperationProcessor))]
    [ProducesResponseType(typeof(AddressBalanceResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressBalanceResponseModel>> GetAddressBalanceByToken([FromRoute] Address address,
                                                                                          [FromRoute] Address token,
                                                                                          CancellationToken cancellationToken)
    {
        var balance = await _mediator.Send(new GetAddressBalanceByTokenQuery(address, token), cancellationToken);
        var response = _mapper.Map<AddressBalanceResponseModel>(balance);
        return Ok(response);
    }

    /// <summary>Refresh Balance</summary>
    /// <remarks>Retrieves and indexes the latest balance of a token for an address.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="token">Address of the token to refresh the balance of, or CRS.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Address balance summary.</returns>
    [HttpPost("{address}/balance/{token}")]
    [OpenApiOperationProcessor(typeof(RefreshAddressBalanceOperationProcessor))]
    [ProducesResponseType(typeof(AddressBalanceResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressBalanceResponseModel>> RefreshAddressBalance([FromRoute] Address address,
                                                                                       [FromRoute] Address token,
                                                                                       CancellationToken cancellationToken)
    {
        var balance = await _mediator.Send(new CreateRefreshAddressBalanceCommand(address, token), cancellationToken);
        var response = _mapper.Map<AddressBalanceResponseModel>(balance);
        return Ok(response);
    }


    /// <summary>Get Mining Positions</summary>
    /// <remarks>Retrieves the mining position of an address in all mining pools.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Mining position summaries</returns>
    [HttpGet("{address}/mining")]
    [OpenApiOperationProcessor(typeof(GetMiningPositionsOperationProcessor))]
    [ProducesResponseType(typeof(MiningPositionsResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MiningPositionsResponseModel>> GetMiningPositions([FromRoute] Address address,
                                                                                     [FromQuery] MiningPositionFilterParameters filters,
                                                                                     CancellationToken cancellationToken)
    {
        var positions = await _mediator.Send(new GetMiningPositionsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        var response = _mapper.Map<MiningPositionsResponseModel>(positions);
        return Ok(response);
    }

    /// <summary>Get Mining Position</summary>
    /// <remarks>Retrieves the mining position of an address in a particular pool.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="miningPool">The address of the mining pool.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Mining position summary</returns>
    [HttpGet("{address}/mining/{miningPool}")]
    [OpenApiOperationProcessor(typeof(GetMiningPositionByPoolOperationProcessor))]
    [ProducesResponseType(typeof(MiningPositionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MiningPositionResponseModel>> GetMiningPositionByPool([FromRoute] Address address,
                                                                                         [FromRoute] Address miningPool,
                                                                                         CancellationToken cancellationToken)
    {
        var position = await _mediator.Send(new GetMiningPositionByPoolQuery(address, miningPool), cancellationToken);
        var response = _mapper.Map<MiningPositionResponseModel>(position);
        return Ok(response);
    }

    /// <summary>Get Staking Positions</summary>
    /// <remarks>Retrieves the staking position of an address in all staking pools.</remarks>
    /// <param name="address">Address of the wallet.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Staking position summaries</returns>
    [HttpGet("{address}/staking")]
    [OpenApiOperationProcessor(typeof(GetStakingPositionsOperationProcessor))]
    [ProducesResponseType(typeof(StakingPositionsResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StakingPositionsResponseModel>> GetStakingPositions([FromRoute] Address address,
                                                                                       [FromQuery] StakingPositionFilterParameters filters,
                                                                                       CancellationToken cancellationToken)
    {
        var positions = await _mediator.Send(new GetStakingPositionsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        var response = _mapper.Map<StakingPositionsResponseModel>(positions);
        return Ok(response);
    }

    /// <summary>Get Staking Position</summary>
    /// <remarks>Retrieves the staking position of an address in a particular pool.</remarks>
    /// <param name="address">Address to lookup</param>
    /// <param name="liquidityPool">Liquidity pool to search</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Staking position summary</returns>
    [HttpGet("{address}/staking/{liquidityPool}")]
    [OpenApiOperationProcessor(typeof(GetStakingPositionByPoolOperationProcessor))]
    [ProducesResponseType(typeof(StakingPositionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StakingPositionResponseModel>> GetStakingPositionByPool([FromRoute] Address address,
                                                                                           [FromRoute] Address liquidityPool,
                                                                                           CancellationToken cancellationToken)
    {
        var position = await _mediator.Send(new GetStakingPositionByPoolQuery(address, liquidityPool), cancellationToken);
        var response = _mapper.Map<StakingPositionResponseModel>(position);
        return Ok(response);
    }
}
