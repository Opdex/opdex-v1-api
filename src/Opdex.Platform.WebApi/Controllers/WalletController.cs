using System;
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
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IApplicationContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public WalletController(IApplicationContext context, IMapper mapper, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{address}/allowance/approved/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovedAllowanceResponseModel>> GetApprovedAllowanceForToken(string address, string token, CancellationToken cancellationToken)
        {
            var allowance = await _mediator.Send(new GetAddressAllowanceForTokenQuery(token, _context.Wallet, address), cancellationToken);
            return _mapper.Map<ApprovedAllowanceResponseModel>(allowance);
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
            var mining = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, walletAddress, findOrThrow: false), cancellationToken);

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
