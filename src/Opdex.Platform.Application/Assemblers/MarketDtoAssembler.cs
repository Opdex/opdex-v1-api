using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class MarketDtoAssembler : IModelAssembler<Market, MarketDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private const SnapshotType SnapshotType = Common.SnapshotType.Daily;

        public MarketDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MarketDto> Assemble(Market market)
        {
            var stakingToken = market.IsStakingMarket
                ? await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId.GetValueOrDefault()))
                : null;

            // Todo: Get multiple snapshots instead
            var currentMarketSnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(market.Id, DateTime.UtcNow, SnapshotType));
            var previousMarketSnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(market.Id, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), SnapshotType));

            var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));

            var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, DateTime.UtcNow, SnapshotType));

            var marketDto = _mapper.Map<MarketDto>(market);
            marketDto.Summary = _mapper.Map<MarketSnapshotDto>(currentMarketSnapshot);

            // Adjust daily change values
            marketDto.Summary.Staking.SetDailyChange(previousMarketSnapshot?.Staking?.Weight);

            if (previousMarketSnapshot?.Liquidity > 0)
            {
                var usdDailyChange = (currentMarketSnapshot.Liquidity - previousMarketSnapshot.Liquidity) / previousMarketSnapshot.Liquidity * 100;
                marketDto.Summary.LiquidityDailyChange = Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
            }

            marketDto.CrsToken = _mapper.Map<TokenDto>(crs);
            marketDto.CrsToken.Summary = _mapper.Map<TokenSnapshotDto>(crsSnapshot);

            if (stakingToken == null) return marketDto;

            marketDto.StakingToken = _mapper.Map<TokenDto>(stakingToken);

            return marketDto;
        }
    }
}
