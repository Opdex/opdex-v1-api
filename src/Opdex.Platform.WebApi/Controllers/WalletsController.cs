using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Controllers
{
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
        /// <param name="address">The address of the wallet.</param>
        /// <param name="token">The address of the token.</param>
        /// <param name="spender">The address for the spender of the allowance.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="ApprovedAllowanceResponseModel"/> summary</returns>
        [HttpGet("{address}/allowance/{token}/approved/{spender}")]
        [ProducesResponseType(typeof(ApprovedAllowanceResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApprovedAllowanceResponseModel>> GetAllowance([FromRoute] Address address,
                                                                                     [FromRoute] Address token,
                                                                                     [FromRoute] Address spender,
                                                                                     CancellationToken cancellationToken)
        {
            var allowances = await _mediator.Send(new GetAddressAllowanceQuery(address, spender, token), cancellationToken);
            var response = _mapper.Map<ApprovedAllowanceResponseModel>(allowances);
            return Ok(response);
        }

        /// <summary>Get Balances</summary>
        /// <remarks>Retrieves token balances for an address.</remarks>
        /// <param name="address">The address of the wallet.</param>
        /// <param name="filters">Filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of address balance summaries by token.</returns>
        [HttpGet("{address}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AddressBalanceResponseModel>>> GetAddressBalances([FromRoute] Address address,
                                                                                                     [FromQuery] AddressBalanceFilterParameters filters,
                                                                                                     CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new GetAddressBalancesWithFilterQuery(address, filters.BuildCursor()), cancellationToken);

            var response = _mapper.Map<AddressBalancesResponseModel>(balances);

            return Ok(response);
        }

        /// <summary>Get Balance</summary>
        /// <remarks>Retrieves a wallet public key balance of a token.</remarks>
        /// <param name="address">The address of the wallet.</param>
        /// <param name="token">The token to get the balance of.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="AddressBalanceResponseModel"/> balance summary</returns>
        [HttpGet("{address}/balance/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressBalanceResponseModel>> GetAddressBalanceByToken([FromRoute] Address address,
                                                                                              [FromRoute] Address token,
                                                                                              CancellationToken cancellationToken)
        {
            var balance = await _mediator.Send(new GetAddressBalanceByTokenQuery(address, token), cancellationToken);
            var response = _mapper.Map<AddressBalanceResponseModel>(balance);
            return Ok(response);
        }

        /// <summary>Get Mining Positions</summary>
        /// <remarks>Retrieves the mining position of an address in all mining pools</remarks>
        /// <param name="address">The address of the wallet.</param>
        /// <param name="filters">Filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Staking position summaries</returns>
        /// <returns></returns>
        [HttpGet("{address}/mining")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<StakingPositionResponseModel>>> GetMiningPositions([FromRoute] Address address,
                                                                                                      [FromQuery] MiningPositionFilterParameters filters,
                                                                                                      CancellationToken cancellationToken)
        {
            var positions = await _mediator.Send(new GetMiningPositionsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
            var response = _mapper.Map<MiningPositionsResponseModel>(positions);
            return Ok(response);
        }

        /// <summary>Get Mining Position</summary>
        /// <remarks>Retrieves the mining position of an address in a particular pool</remarks>
        /// <param name="address">The address of the wallet.</param>
        /// <param name="miningPool">The address of the mining pool.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Mining position summary</returns>
        [HttpGet("{address}/mining/{miningPool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <remarks>Retrieves the staking position of an address in all staking pools</remarks>
        /// <param name="address">The address of the wallet.</param>
        /// <param name="filters">Filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Staking position summaries</returns>
        [HttpGet("{address}/staking")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<StakingPositionResponseModel>>> GetStakingPositions([FromRoute] Address address,
                                                                                                       [FromQuery] StakingPositionFilterParameters filters,
                                                                                                       CancellationToken cancellationToken)
        {
            var positions = await _mediator.Send(new GetStakingPositionsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
            var response = _mapper.Map<StakingPositionsResponseModel>(positions);
            return Ok(response);
        }

        /// <summary>Get Staking Position</summary>
        /// <remarks>Retrieves the staking position of an address in a particular pool</remarks>
        /// <param name="address">Address to lookup</param>
        /// <param name="liquidityPool">Liquidity pool to search</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Staking position summary</returns>
        [HttpGet("{address}/staking/{liquidityPool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
}