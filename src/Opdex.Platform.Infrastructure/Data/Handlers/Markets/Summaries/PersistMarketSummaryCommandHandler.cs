using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;

public class PersistMarketSummaryCommandHandler : IRequestHandler<PersistMarketSummaryCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market_summary (
                {nameof(MarketSummaryEntity.MarketId)},
                {nameof(MarketSummaryEntity.LiquidityUsd)},
                {nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)},
                {nameof(MarketSummaryEntity.VolumeUsd)},
                {nameof(MarketSummaryEntity.StakingWeight)},
                {nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)},
                {nameof(MarketSummaryEntity.StakingUsd)},
                {nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)},
                {nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)},
                {nameof(MarketSummaryEntity.MarketRewardsDailyUsd)},
                {nameof(MarketSummaryEntity.CreatedBlock)},
                {nameof(MarketSummaryEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MarketSummaryEntity.MarketId)},
                @{nameof(MarketSummaryEntity.LiquidityUsd)},
                @{nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)},
                @{nameof(MarketSummaryEntity.VolumeUsd)},
                @{nameof(MarketSummaryEntity.StakingWeight)},
                @{nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)},
                @{nameof(MarketSummaryEntity.StakingUsd)},
                @{nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)},
                @{nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)},
                @{nameof(MarketSummaryEntity.MarketRewardsDailyUsd)},
                @{nameof(MarketSummaryEntity.CreatedBlock)},
                @{nameof(MarketSummaryEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE market_summary
                SET
                    {nameof(MarketSummaryEntity.LiquidityUsd)} = @{nameof(MarketSummaryEntity.LiquidityUsd)},
                    {nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)} = @{nameof(MarketSummaryEntity.DailyLiquidityUsdChangePercent)},
                    {nameof(MarketSummaryEntity.VolumeUsd)} = @{nameof(MarketSummaryEntity.VolumeUsd)},
                    {nameof(MarketSummaryEntity.StakingWeight)} = @{nameof(MarketSummaryEntity.StakingWeight)},
                    {nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)} = @{nameof(MarketSummaryEntity.DailyStakingWeightChangePercent)},
                    {nameof(MarketSummaryEntity.StakingUsd)} = @{nameof(MarketSummaryEntity.StakingUsd)},
                    {nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)} = @{nameof(MarketSummaryEntity.DailyStakingUsdChangePercent)},
                    {nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)} = @{nameof(MarketSummaryEntity.ProviderRewardsDailyUsd)},
                    {nameof(MarketSummaryEntity.MarketRewardsDailyUsd)} = @{nameof(MarketSummaryEntity.MarketRewardsDailyUsd)},
                    {nameof(MarketSummaryEntity.ModifiedBlock)} = @{nameof(MarketSummaryEntity.ModifiedBlock)}
                WHERE {nameof(MarketSummaryEntity.Id)} = @{nameof(MarketSummaryEntity.Id)};"
            .RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistMarketSummaryCommandHandler(IDbContext context, IMapper mapper,
                                                     ILogger<PersistMarketSummaryCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistMarketSummaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MarketSummaryEntity>(request.Summary);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                { "MarketId" , request.Summary.MarketId},
                { "BlockHeight" , request.Summary.ModifiedBlock}
            }))
            {
                _logger.LogError(ex, "Unable to persist market summary.");
            }
            return 0;
        }
    }
}
