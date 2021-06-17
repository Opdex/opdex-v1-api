using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Assemblers
{
    public class LiquidityPoolDtoAssembler: IModelAssembler<LiquidityPool, LiquidityPoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LiquidityPoolDto> Assemble(LiquidityPool pool)
        {
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId, findOrThrow: true));

            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));
            // Todo: Check stale (shouldn't be)
            var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, DateTime.UtcNow, SnapshotType.Minute));

            var stakingTokenUsd = 0m;
            if (market.StakingTokenId > 0)
            {
                var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(pool.SrcTokenId, pool.MarketId, DateTime.UtcNow, SnapshotType.Hourly));

                if (stakingTokenSnapshot.EndDate < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)))
                {
                    // stakingTokenSnapshot.ResetStaleSnapshot();
                }

                stakingTokenUsd = stakingTokenSnapshot.Price.Close;
            }

            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId));
            var poolSnapshotQuery = new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, DateTime.UtcNow, SnapshotType.Daily);
            var poolSnapshot = await _mediator.Send(poolSnapshotQuery);

            // Update stale snapshots if necessary
            if (poolSnapshot.EndDate < DateTime.UtcNow)
            {
                // Todo: Fix, need to untangle and get SrcSnapshot first
                // poolSnapshot.ResetStaleSnapshot(crsSnapshot.Price.Close, stakingTokenUsd, DateTime.UtcNow);
            }

            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);
            var tokenDto = _mapper.Map<TokenDto>(srcToken);
            var summaryDto = _mapper.Map<LiquidityPoolSnapshotDto>(poolSnapshot);

            poolDto.Token = tokenDto;
            poolDto.Summary = summaryDto;

            if (market.StakingTokenId > 0 && pool.SrcTokenId != market.StakingTokenId)
            {
                poolDto.StakingEnabled = true;

                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, findOrThrow: false));

                // Todo: Add Mining Pool Dto and Response models
                // Todo: Needs to have a not null mining pool and be active.
                poolDto.MiningEnabled = miningPool != null;
            }

            return poolDto;
        }
    }
}