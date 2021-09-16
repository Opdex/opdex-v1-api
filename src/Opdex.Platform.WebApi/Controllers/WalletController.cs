using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public WalletController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>Get Approved Allowance</summary>
        /// <remarks>Retrieve the allowance of a spender for tokens owned by another wallet.</remarks>
        /// <param name="address">The owner's wallet address of the tokens.</param>
        /// <param name="token">The token address.</param>
        /// <param name="spender">The spender of the allowance.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="ApprovedAllowanceResponseModel"/> summary</returns>
        [HttpGet("{address}/allowance/{token}/approved/{spender}")]
        [ProducesResponseType(typeof(ApprovedAllowanceResponseModel), StatusCodes.Status200OK)]
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
        /// <param name="address">The address to lookup balances for.</param>
        /// <param name="tokens">Specific tokens to lookup.</param>
        /// <param name="includeLpTokens">Includes all tokens if true, otherwise excludes liquidity pool tokens.</param>
        /// <param name="includeZeroBalances">Only includes 0 balances if true.</param>
        /// <param name="direction">Order in which to sort results.</param>
        /// <param name="limit">Number of results to take per page.</param>
        /// <param name="cursor">Cursor for pagination.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of address balance summaries by token.</returns>
        [HttpGet("{address}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AddressBalanceResponseModel>>> GetAddressBalances([FromRoute] Address address,
                                                                                                     [FromQuery] IEnumerable<Address> tokens,
                                                                                                     [FromQuery] bool? includeLpTokens,
                                                                                                     [FromQuery] bool? includeZeroBalances,
                                                                                                     [FromQuery] SortDirectionType direction,
                                                                                                     [FromQuery] uint limit,
                                                                                                     [FromQuery] string cursor,
                                                                                                     CancellationToken cancellationToken)
        {
            AddressBalancesCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !AddressBalancesCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new AddressBalancesCursor(tokens, includeLpTokens ?? true, includeZeroBalances ?? false, direction, limit, PagingDirection.Forward, default);
            }

            var balances = await _mediator.Send(new GetAddressBalancesWithFilterQuery(address, pagingCursor), cancellationToken);

            var response = _mapper.Map<AddressBalancesResponseModel>(balances);

            return Ok(response);
        }

        /// <summary>Get Balance</summary>
        /// <remarks>Retrieves a wallet public key balance of a token.</remarks>
        /// <param name="address">The wallet address.</param>
        /// <param name="token">The token to get the balance of.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="AddressBalanceResponseModel"/> balance summary</returns>
        [HttpGet("{address}/balance/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <param name="address">Address to lookup</param>
        /// <param name="liquidityPools">Specific liquidity pools to include.</param>
        /// <param name="miningPools">Specific mining pools to include.</param>
        /// <param name="includeZeroAmounts">Only includes 0 amounts if true.</param>
        /// <param name="direction">Order in which to sort results.</param>
        /// <param name="limit">Number of results to take per page.</param>
        /// <param name="cursor">Cursor for pagination.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Staking position summaries</returns>
        /// <returns></returns>
        [HttpGet("{address}/mining")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StakingPositionResponseModel>>> GetMiningPositions([FromRoute] Address address,
                                                                                                      [FromQuery] IEnumerable<Address> liquidityPools,
                                                                                                      [FromQuery] IEnumerable<Address> miningPools,
                                                                                                      [FromQuery] bool? includeZeroAmounts,
                                                                                                      [FromQuery] SortDirectionType direction,
                                                                                                      [FromQuery] uint limit,
                                                                                                      [FromQuery] string cursor,
                                                                                                      CancellationToken cancellationToken)
        {
            MiningPositionsCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !MiningPositionsCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new MiningPositionsCursor(liquidityPools, miningPools, includeZeroAmounts ?? false, direction, limit, PagingDirection.Forward, default);
            }

            var positions = await _mediator.Send(new GetMiningPositionsWithFilterQuery(address, pagingCursor), cancellationToken);
            var response = _mapper.Map<MiningPositionsResponseModel>(positions);
            return Ok(response);
        }

        /// <summary>Get Mining Position</summary>
        /// <remarks>Retrieves the mining position of an address in a particular pool</remarks>
        /// <param name="address">Address to lookup</param>
        /// <param name="miningPool">Mining pool to search</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Mining position summary</returns>
        [HttpGet("{address}/mining/{miningPool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <param name="address">Address to lookup</param>
        /// <param name="liquidityPools">Specific liquidity pools to include.</param>
        /// <param name="includeZeroAmounts">Only includes 0 amounts if true.</param>
        /// <param name="direction">Order in which to sort results.</param>
        /// <param name="limit">Number of results to take per page.</param>
        /// <param name="cursor">Cursor for pagination.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Staking position summaries</returns>
        [HttpGet("{address}/staking")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StakingPositionResponseModel>>> GetStakingPositions([FromRoute] Address address,
                                                                                                       [FromQuery] IEnumerable<Address> liquidityPools,
                                                                                                       [FromQuery] bool? includeZeroAmounts,
                                                                                                       [FromQuery] SortDirectionType direction,
                                                                                                       [FromQuery] uint limit,
                                                                                                       [FromQuery] string cursor,
                                                                                                       CancellationToken cancellationToken)
        {
            StakingPositionsCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !StakingPositionsCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new StakingPositionsCursor(liquidityPools, includeZeroAmounts ?? false, direction, limit, PagingDirection.Forward, default);
            }

            var positions = await _mediator.Send(new GetStakingPositionsWithFilterQuery(address, pagingCursor), cancellationToken);
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
