using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;

public class PersistLiquidityPoolSummaryCommandHandler : IRequestHandler<PersistLiquidityPoolSummaryCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO pool_liquidity_summary (
                {nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)},
                {nameof(LiquidityPoolSummaryEntity.LiquidityUsd)},
                {nameof(LiquidityPoolSummaryEntity.DailyLiquidityUsdChangePercent)},
                {nameof(LiquidityPoolSummaryEntity.VolumeUsd)},
                {nameof(LiquidityPoolSummaryEntity.StakingWeight)},
                {nameof(LiquidityPoolSummaryEntity.DailyStakingWeightChangePercent)},
                {nameof(LiquidityPoolSummaryEntity.LockedCrs)},
                {nameof(LiquidityPoolSummaryEntity.LockedSrc)},
                {nameof(LiquidityPoolSummaryEntity.CreatedBlock)},
                {nameof(LiquidityPoolSummaryEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(LiquidityPoolSummaryEntity.LiquidityPoolId)},
                @{nameof(LiquidityPoolSummaryEntity.LiquidityUsd)},
                @{nameof(LiquidityPoolSummaryEntity.DailyLiquidityUsdChangePercent)},
                @{nameof(LiquidityPoolSummaryEntity.VolumeUsd)},
                @{nameof(LiquidityPoolSummaryEntity.StakingWeight)},
                @{nameof(LiquidityPoolSummaryEntity.DailyStakingWeightChangePercent)},
                @{nameof(LiquidityPoolSummaryEntity.LockedCrs)},
                @{nameof(LiquidityPoolSummaryEntity.LockedSrc)},
                @{nameof(LiquidityPoolSummaryEntity.CreatedBlock)},
                @{nameof(LiquidityPoolSummaryEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE pool_liquidity_summary
                SET
                    {nameof(LiquidityPoolSummaryEntity.LiquidityUsd)} = @{nameof(LiquidityPoolSummaryEntity.LiquidityUsd)},
                    {nameof(LiquidityPoolSummaryEntity.DailyLiquidityUsdChangePercent)} = @{nameof(LiquidityPoolSummaryEntity.DailyLiquidityUsdChangePercent)},
                    {nameof(LiquidityPoolSummaryEntity.VolumeUsd)} = @{nameof(LiquidityPoolSummaryEntity.VolumeUsd)},
                    {nameof(LiquidityPoolSummaryEntity.StakingWeight)} = @{nameof(LiquidityPoolSummaryEntity.StakingWeight)},
                    {nameof(LiquidityPoolSummaryEntity.DailyStakingWeightChangePercent)} = @{nameof(LiquidityPoolSummaryEntity.DailyStakingWeightChangePercent)},
                    {nameof(LiquidityPoolSummaryEntity.LockedCrs)} = @{nameof(LiquidityPoolSummaryEntity.LockedCrs)},
                    {nameof(LiquidityPoolSummaryEntity.LockedSrc)} = @{nameof(LiquidityPoolSummaryEntity.LockedSrc)},
                    {nameof(LiquidityPoolSummaryEntity.ModifiedBlock)} = @{nameof(LiquidityPoolSummaryEntity.ModifiedBlock)}
                WHERE {nameof(LiquidityPoolSummaryEntity.Id)} = @{nameof(LiquidityPoolSummaryEntity.Id)};"
            .RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistLiquidityPoolSummaryCommandHandler(IDbContext context, IMapper mapper,
                                                     ILogger<PersistLiquidityPoolSummaryCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistLiquidityPoolSummaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<LiquidityPoolSummaryEntity>(request.Summary);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "LiquidityPoolId" , request.Summary.LiquidityPoolId},
                { "BlockHeight" , request.Summary.ModifiedBlock}
            }))
            {
                _logger.LogError(ex, $"Unable to persist liquidity pool summary.");
            }
            return 0;
        }
    }
}