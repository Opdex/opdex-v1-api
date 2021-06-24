using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using System.Linq;

namespace Opdex.Platform.Application.Assemblers
{
    public class LiquidityPoolDtoAssembler : IModelAssembler<LiquidityPool, LiquidityPoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private const SnapshotType SnapshotType = Common.SnapshotType.Daily;

        public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LiquidityPoolDto> Assemble(LiquidityPool pool)
        {
            var now = DateTime.UtcNow.ToEndOf(SnapshotType);
            var yesterday = now.Subtract(TimeSpan.FromDays(1)).ToStartOf(SnapshotType);
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId));
            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));
            var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, now, SnapshotType));

            Token stakingToken = null;
            TokenSnapshot stakingTokenSnapshot = null;

            if (market.StakingTokenId > 0)
            {
                var stakingTokenId = market.StakingTokenId.GetValueOrDefault();

                stakingToken = await _mediator.Send(new RetrieveTokenByIdQuery(stakingTokenId));
                stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingTokenId, pool.MarketId, now, SnapshotType));
            }

            // SRC token and snapshot details
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId));
            var srcTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(srcToken.Id, market.Id, now, SnapshotType));

            // LP token and snapshot details
            var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.LpTokenId));
            var lpTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(lpToken.Id, market.Id, now, SnapshotType));

            // LP pool snapshot details
            var liquidityPoolSnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, yesterday, now, SnapshotType));
            var poolSnapshots = liquidityPoolSnapshots.ToList();

            // Todo: Should we throw here?? What happens if right now doesn't have a snapshot for some reason but historically
            // there are available snapshots. Returning new LiquidityPoolSnapshot wouldn't be the correct answer
            var currentPoolSnapshot = poolSnapshots.FirstOrDefault() ?? new LiquidityPoolSnapshot(pool.Id, SnapshotType, now);
            var previousPoolSnapshot = poolSnapshots.LastOrDefault();

            // Map to Dtos
            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);

            poolDto.Summary = _mapper.Map<LiquidityPoolSnapshotDto>(currentPoolSnapshot);

            // Todo: Standardize this entire process
            if (previousPoolSnapshot != null && previousPoolSnapshot.Reserves.Usd != 0)
            {
                var currentReserves = currentPoolSnapshot.Reserves.Usd;
                var previousReserves = previousPoolSnapshot.Reserves.Usd;
                var usdDailyChange = (currentReserves - previousReserves) / previousReserves * 100;

                poolDto.Summary.Reserves.UsdDailyChange = Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                poolDto.Summary.Reserves.UsdDailyChange = 0.00m;
            }

            if (previousPoolSnapshot != null && previousPoolSnapshot.Staking.Weight != "0")
            {
                const int decimals = TokenConstants.Opdex.Decimals;
                var currentWeight = currentPoolSnapshot.Staking.Weight.ToRoundedDecimal(decimals, decimals);
                var previousWeight = previousPoolSnapshot.Staking.Weight.ToRoundedDecimal(decimals, decimals);
                var weightDailyChange = (currentWeight - previousWeight) / previousWeight * 100;

                poolDto.Summary.Staking.WeightDailyChange = Math.Round(weightDailyChange, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                poolDto.Summary.Staking.WeightDailyChange = 0.00m;
            }

            poolDto.Summary.SrcTokenDecimals = srcToken.Decimals;
            poolDto.SrcToken = _mapper.Map<TokenDto>(srcToken);
            poolDto.SrcToken.Summary = _mapper.Map<TokenSnapshotDto>(srcTokenSnapshot);
            poolDto.LpToken = _mapper.Map<TokenDto>(lpToken);
            poolDto.LpToken.Summary = _mapper.Map<TokenSnapshotDto>(lpTokenSnapshot);
            poolDto.CrsToken = _mapper.Map<TokenDto>(crsToken);
            poolDto.CrsToken.Summary = _mapper.Map<TokenSnapshotDto>(crsSnapshot);

            // Update mining/staking flags
            if (stakingToken != null && pool.SrcTokenId != market.StakingTokenId)
            {
                poolDto.StakingEnabled = true;
                poolDto.StakingToken = _mapper.Map<TokenDto>(stakingToken);
                poolDto.StakingToken.Summary = _mapper.Map<TokenSnapshotDto>(stakingTokenSnapshot);

                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, false));

                // Todo: Add Mining Pool Dto and Response models
                // Todo: Needs to have a not null mining pool and be active.
                poolDto.MiningEnabled = miningPool != null;
            }
            else
            {
                poolDto.Summary.Staking = null;
            }

            return poolDto;
        }
    }
}
