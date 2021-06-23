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
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Assemblers
{
    public class LiquidityPoolDtoAssembler : IModelAssembler<LiquidityPool, LiquidityPoolDto>
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
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId));

            var crsToken = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));
            var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crsToken.Id, 0, DateTime.UtcNow, SnapshotType.Daily));

            Token stakingToken = null;
            TokenSnapshot stakingTokenSnapshot = null;

            if (market.StakingTokenId > 0)
            {
                var stakingTokenId = market.StakingTokenId.GetValueOrDefault();

                stakingToken = await _mediator.Send(new RetrieveTokenByIdQuery(stakingTokenId));

                stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingTokenId,
                                                                                                         pool.MarketId,
                                                                                                         DateTime.UtcNow,
                                                                                                         SnapshotType.Daily));
            }

            // SRC token and snapshot details
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId));
            var srcTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(srcToken.Id,
                                                                                                 market.Id,
                                                                                                 DateTime.UtcNow,
                                                                                                 SnapshotType.Daily));

            // LP token and snapshot details
            var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.LpTokenId));
            var lpTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(lpToken.Id,
                                                                                                market.Id,
                                                                                                DateTime.UtcNow,
                                                                                                SnapshotType.Daily));

            // LP pool snapshot details
            var poolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id,
                                                                                                     DateTime.UtcNow,
                                                                                                     SnapshotType.Daily));

            // Map to Dtos
            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);

            poolDto.Summary =  _mapper.Map<LiquidityPoolSnapshotDto>(poolSnapshot);
            poolDto.Summary.SrcTokenDecimals = srcToken.Decimals;
            poolDto.SrcToken = _mapper.Map<TokenDto>(srcToken);
            poolDto.SrcToken.Summary = _mapper.Map<TokenSnapshotDto>(srcTokenSnapshot);
            poolDto.LpToken = _mapper.Map<TokenDto>(lpToken);
            poolDto.LpToken.Summary = _mapper.Map<TokenSnapshotDto>(lpTokenSnapshot);
            poolDto.CrsToken = _mapper.Map<TokenDto>(crsToken);
            poolDto.CrsToken.Summary = _mapper.Map<TokenSnapshotDto>(crsSnapshot);

            if (stakingToken != null)
            {
                poolDto.StakingToken = _mapper.Map<TokenDto>(stakingToken);
                poolDto.StakingToken.Summary = _mapper.Map<TokenSnapshotDto>(stakingTokenSnapshot);
            }

            // Update mining/staking flags
            if (market.StakingTokenId > 0 && pool.SrcTokenId != market.StakingTokenId)
            {
                poolDto.StakingEnabled = true;

                var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, false));

                // Todo: Add Mining Pool Dto and Response models
                // Todo: Needs to have a not null mining pool and be active.
                poolDto.MiningEnabled = miningPool != null;
            }

            return poolDto;
        }
    }
}
