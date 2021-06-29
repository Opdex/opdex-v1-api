using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
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
        private readonly IModelAssembler<MiningPool, MiningPoolDto> _miningPoolAssembler;

        private const SnapshotType SnapshotType = Common.SnapshotType.Daily;

        public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper, IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
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

            // Todo: If Stale
            // if (srcTokenSnapshot.EndDate < now)
            // {
            //     srcTokenSnapshot.ResetStaleSnapshot();
            // }

            // LP token and snapshot details
            var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.LpTokenId));
            var lpTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(lpToken.Id, market.Id, now, SnapshotType));

            // Todo: If Stale
            // if (lpTokenSnapshot.EndDate < now)
            // {
            //     lpTokenSnapshot.ResetStaleSnapshot();
            // }

            // LP pool snapshot details
            var liquidityPoolSnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, yesterday, now, SnapshotType));
            var poolSnapshots = liquidityPoolSnapshots.ToList();

            // Get the current snapshot from the list, when null, retrieve the last possible snapshot or a new one entirely
            var currentPoolSnapshot = poolSnapshots.FirstOrDefault();

            // If we keep this block, its essentially a fallback for forks/reorgs if today's snapshot (which should exist at all times), doesnt
            if (currentPoolSnapshot == null)
            {
                var latest = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, now, SnapshotType));
                if (latest.EndDate < now)
                {
                    var stakingTokenPrice = stakingTokenSnapshot?.Price?.Close ?? 0.00m;
                    latest.ResetStaleSnapshot(crsSnapshot.Price.Close, srcTokenSnapshot.Price.Close, stakingTokenPrice, srcToken.Decimals, now);
                }
            }

            var previousPoolSnapshot = poolSnapshots.LastOrDefault();

            // Map to Dtos
            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);

            poolDto.Summary = _mapper.Map<LiquidityPoolSnapshotDto>(currentPoolSnapshot);

            // adjust daily change values
            poolDto.Summary.Reserves.SetUsdDailyChange(previousPoolSnapshot?.Reserves?.Usd ?? 0.00m);
            poolDto.Summary.Staking.SetDailyChange(previousPoolSnapshot?.Staking?.Weight);

            // Todo: Revisit - sets decimals value, dirty hack to mapping in the web api layer
            poolDto.Summary.SrcTokenDecimals = srcToken.Decimals;

            // src
            poolDto.SrcToken = _mapper.Map<TokenDto>(srcToken);
            poolDto.SrcToken.Summary = _mapper.Map<TokenSnapshotDto>(srcTokenSnapshot);

            // lp
            poolDto.LpToken = _mapper.Map<TokenDto>(lpToken);
            poolDto.LpToken.Summary = _mapper.Map<TokenSnapshotDto>(lpTokenSnapshot);

            // crs
            poolDto.CrsToken = _mapper.Map<TokenDto>(crsToken);
            poolDto.CrsToken.Summary = _mapper.Map<TokenSnapshotDto>(crsSnapshot);

            // Update mining/staking flags
            if (stakingToken != null && pool.SrcTokenId != market.StakingTokenId)
            {
                poolDto.StakingEnabled = true;
                poolDto.StakingToken = _mapper.Map<TokenDto>(stakingToken);
                poolDto.StakingToken.Summary = _mapper.Map<TokenSnapshotDto>(stakingTokenSnapshot);

                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id));
                poolDto.MiningPool = await _miningPoolAssembler.Assemble(miningPool);
            }
            else
            {
                poolDto.Summary.Staking = null;
            }

            return poolDto;
        }
    }
}
