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
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
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

        /// <summary>
        /// Retrieves a collection of approved allowances
        /// </summary>
        /// <param name="address">Wallet owner address</param>
        /// <param name="spender">Spender address to filter by</param>
        /// <param name="token">Token address to filter by</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Collection of approved allowances</returns>
        [HttpGet("{address}/allowance/approved")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApprovedAllowances(string address, [FromQuery] string spender, [FromQuery] string token, CancellationToken cancellationToken)
        {
            var allowances = await _mediator.Send(new GetAddressAllowancesApprovedByOwnerQuery(address, spender, token), cancellationToken);
            var response = _mapper.Map<IEnumerable<ApprovedAllowanceResponseModel>>(allowances);
            return Ok(response);
        }

        /// <summary>
        /// Retrieve the allowance of a spender for tokens owned by another wallet.
        /// </summary>
        /// <param name="address">The owner's wallet address of the tokens.</param>
        /// <param name="token">The token address.</param>
        /// <param name="spender">The spender of the allowance.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="ApprovedAllowanceResponseModel"/> summary</returns>
        [HttpGet("{address}/allowance/{token}/approved/{spender}")]
        [ProducesResponseType(typeof(ApprovedAllowanceResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovedAllowanceResponseModel>> GetAllowance(string address, string token, string spender, CancellationToken cancellationToken)
        {
            var allowances = await _mediator.Send(new GetAddressAllowanceQuery(address, spender, token), cancellationToken);
            var response = _mapper.Map<ApprovedAllowanceResponseModel>(allowances);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a wallet public key balance of a token.
        /// </summary>
        /// <param name="address">The wallet address.</param>
        /// <param name="token">The token to get the balance of.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="AddressBalanceResponseModel"/> balance summary</returns>
        [HttpGet("{address}/balance/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressBalanceResponseModel>> GetAddressBalanceByToken(string address, string token, CancellationToken cancellationToken)
        {
            var balance = await _mediator.Send(new GetAddressBalanceByTokenQuery(address, token), cancellationToken);
            var response = _mapper.Map<AddressBalanceResponseModel>(balance);
            return Ok(response);
        }

        [HttpGet("{address}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AddressBalanceResponseModel>>> GetAddressBalances(string address,
                                                                                                     [FromQuery] IEnumerable<string> tokens,
                                                                                                     [FromQuery] bool? includeLpTokens,
                                                                                                     [FromQuery] bool? includeZeroBalances,
                                                                                                     [FromQuery] SortDirectionType direction,
                                                                                                     [FromQuery] uint limit,
                                                                                                     [FromQuery] string next,
                                                                                                     [FromQuery] string previous,
                                                                                                     CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new GetAddressBalancesWithFilterQuery(address, tokens, includeLpTokens ?? true, includeZeroBalances ?? false,
                                                                                      direction, limit, next, previous), cancellationToken);

            var response = _mapper.Map<AddressBalancesResponseModel>(balances);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves the staking position of an address in a particular pool
        /// </summary>
        /// <param name="address">Address to lookup</param>
        /// <param name="liquidityPool">Liquidity pool to search</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Staking position summary</returns>
        [HttpGet("{address}/staking/{liquidityPool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StakingPositionResponseModel>> GetStakingPositionByPool(string address, string liquidityPool, CancellationToken cancellationToken)
        {
            var position = await _mediator.Send(new GetStakingPositionByPoolQuery(address, liquidityPool), cancellationToken);
            var response = _mapper.Map<StakingPositionResponseModel>(position);
            return Ok(response);
        }

        [HttpGet("summary/pool/{poolAddress}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetWalletSummaryByPool(string poolAddress, string walletAddress, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolAddress, findOrThrow: true), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId, findOrThrow: true), cancellationToken);
            var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(walletAddress, token.Id, findOrThrow: false), cancellationToken);
            var crsBalance = 0;
            var lpTokens = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(walletAddress, pool.LpTokenId, findOrThrow: false), cancellationToken);
            var staking = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(pool.Id, walletAddress, findOrThrow: false), cancellationToken);

            var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, findOrThrow: false), cancellationToken);

            AddressMining mining = null;

            if (miningPool != null)
            {
                mining = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, walletAddress, findOrThrow: false), cancellationToken);
            }

            return Ok(new
            {
                SrcBalance = addressBalance?.Balance?.InsertDecimal(token.Decimals) ?? "0.00000000",
                CrsBalance = crsBalance,
                Providing = lpTokens?.Balance?.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals) ?? "0.00000000",
                Staking = staking?.Weight?.InsertDecimal(TokenConstants.Opdex.Decimals) ?? "0.00000000",
                Mining = mining?.Balance?.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals) ?? "0.00000000"
            });
        }
    }
}
