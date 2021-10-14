using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using System.Linq;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Assemblers
{
    public class LiquidityPoolDtoAssembler : IModelAssembler<LiquidityPool, LiquidityPoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<MiningPool, MiningPoolDto> _miningPoolAssembler;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

        public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper, IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler,
                                         IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<LiquidityPoolDto> Assemble(LiquidityPool pool)
        {
            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);

            var now = DateTime.UtcNow.ToEndOf(SnapshotType);
            var yesterday = now.Subtract(TimeSpan.FromDays(1)).ToStartOf(SnapshotType);

            var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId));

            // Set the transaction fee for the pool
            poolDto.TransactionFee = market.TransactionFee == 0
                ? 0
                : Math.Round((decimal)market.TransactionFee / 1000, 3); // 1-10 => .01 - .001 as percent

            // Assemble CRS Token
            poolDto.CrsToken = await AssembleToken(Address.Cirrus, 0);

            // Assemble staking token details when required
            var stakingTokenDto = market.StakingTokenId > 0 ? await AssembleToken(market.StakingTokenId.Value, market.Id) : null;

            // Assemble SRC Token
            poolDto.SrcToken = await AssembleToken(pool.SrcTokenId, market.Id);

            // Assemble LP Token
            poolDto.LpToken = await AssembleToken(pool.LpTokenId, market.Id);

            // LP pool snapshot details
            var liquidityPoolSnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, yesterday, now, SnapshotType));
            var poolSnapshots = liquidityPoolSnapshots.ToList();

            // Get the current snapshot from the list, when null, retrieve the last possible snapshot or a new one entirely
            var currentPoolSnapshot = poolSnapshots.FirstOrDefault();

            // If we keep this block, its essentially a fallback for forks/reorgs if today's snapshot (which should exist at all times), doesnt
            if (currentPoolSnapshot == null)
            {
                currentPoolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, now, SnapshotType));
                if (currentPoolSnapshot.EndDate < now)
                {
                    var stakingTokenPrice = stakingTokenDto?.Summary?.Price?.Close ?? 0.00m;
                    var crsPrice = poolDto.CrsToken.Summary.Price.Close;
                    var srcPrice = poolDto.SrcToken.Summary.Price.Close;

                    currentPoolSnapshot.ResetStaleSnapshot(crsPrice, srcPrice, stakingTokenPrice, poolDto.SrcToken.Sats, now);
                }
            }

            var previousPoolSnapshot = poolSnapshots.LastOrDefault();

            poolDto.Summary = _mapper.Map<LiquidityPoolSnapshotDto>(currentPoolSnapshot);

            // adjust daily change values
            poolDto.Summary.Reserves.SetUsdDailyChange(previousPoolSnapshot?.Reserves?.Usd ?? 0.00m);

            // Todo: Revisit - sets decimals value, dirty hack to mapping in the web api layer
            poolDto.Summary.SrcTokenDecimals = poolDto.SrcToken.Decimals;

            // Update mining/staking flags
            if (stakingTokenDto != null && pool.SrcTokenId != market.StakingTokenId)
            {
                poolDto.StakingEnabled = true;

                // Set assembled staking token
                poolDto.StakingToken = stakingTokenDto;

                // Set staking daily change
                poolDto.Summary.Staking.SetDailyChange(previousPoolSnapshot?.Staking?.Weight ?? UInt256.Zero);

                // Set Nomination Status
                var governance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(stakingTokenDto.Id));
                var nominations = await _mediator.Send(new RetrieveActiveGovernanceNominationsByGovernanceIdQuery(governance.Id));
                poolDto.Summary.Staking.IsNominated = nominations.Any(nomination => nomination.LiquidityPoolId == poolDto.Id);

                // Get mining pool
                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id));

                // Assemble mining pool
                poolDto.MiningPool = await _miningPoolAssembler.Assemble(miningPool);
            }
            else
            {
                poolDto.Summary.Staking = null;
                poolDto.MiningPool = null;
            }

            return poolDto;
        }

        private async Task<TokenDto> AssembleToken(ulong tokenId, ulong marketId)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(tokenId));

            return await AssembleTokenExecute(token, marketId);
        }

        private async Task<TokenDto> AssembleToken(Address tokenAddress, ulong marketId)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress));

            return await AssembleTokenExecute(token, marketId);
        }

        private async Task<TokenDto> AssembleTokenExecute(Token token, ulong marketId)
        {
            token.SetMarket(marketId);

            return await _tokenAssembler.Assemble(token);
        }
    }
}
