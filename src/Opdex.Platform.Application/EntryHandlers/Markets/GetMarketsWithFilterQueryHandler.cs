using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets;

public class GetMarketsWithFilterQueryHandler : EntryFilterQueryHandler<GetMarketsWithFilterQuery, MarketsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<Market, MarketDto> _marketAssembler;

    public GetMarketsWithFilterQueryHandler(IMediator mediator, IModelAssembler<Market, MarketDto> marketAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _marketAssembler = marketAssembler ?? throw new ArgumentNullException(nameof(marketAssembler));
    }

    public override async Task<MarketsDto> Handle(GetMarketsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var markets = await _mediator.Send(new RetrieveMarketsWithFilterQuery(request.Cursor), cancellationToken);

        var dtos = await Task.WhenAll(markets.Select(token => _marketAssembler.Assemble(token)));

        var marketResults = dtos.ToList();

        var cursor = BuildCursorDto(marketResults, request.Cursor, pointerSelector: result =>
        {
            return request.Cursor.OrderBy switch
            {
                MarketOrderByType.LiquidityUsd => (result.Summary.LiquidityUsd, result.Id),
                MarketOrderByType.StakingUsd => (result.Summary.Staking.StakingUsd, result.Id),
                MarketOrderByType.StakingWeight => (result.Summary.Staking.StakingWeight, result.Id),
                MarketOrderByType.VolumeUsd => (result.Summary.VolumeUsd , result.Id),
                MarketOrderByType.MarketRewardsDailyUsd => (result.Summary.Rewards.MarketDailyUsd , result.Id),
                MarketOrderByType.ProviderRewardsDailyUsd => (result.Summary.Rewards.ProviderDailyUsd, result.Id),
                MarketOrderByType.DailyLiquidityUsdChangePercent => (result.Summary.DailyLiquidityUsdChangePercent, result.Id),
                MarketOrderByType.DailyStakingUsdChangePercent => (result.Summary.Staking.DailyStakingUsdChangePercent, result.Id),
                MarketOrderByType.DailyStakingWeightChangePercent => (result.Summary.Staking.DailyStakingWeightChangePercent, result.Id),
                _ => (FixedDecimal.Zero, result.Id)
            };
        });

        return new MarketsDto { Markets = marketResults, Cursor = cursor };
    }
}
