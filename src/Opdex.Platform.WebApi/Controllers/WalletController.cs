using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("summary/pool/{poolAddress}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetWalletSummaryByPool(string poolAddress, string walletAddress, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolAddress, findOrThrow: true), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId, findOrThrow: true), cancellationToken);
            var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(token.Id, walletAddress, findOrThrow: false));
            var crsBalance = 0;
            var lpTokens = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(pool.LpTokenId, walletAddress, findOrThrow: false));
            var staking = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(pool.Id, walletAddress, findOrThrow: false));

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
