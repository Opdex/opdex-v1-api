using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;

public class PersistLiquidityPoolSnapshotCommandHandler : IRequestHandler<PersistLiquidityPoolSnapshotCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO pool_liquidity_snapshot (
                {nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)},
                {nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)},
                {nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                {nameof(LiquidityPoolSnapshotEntity.StartDate)},
                {nameof(LiquidityPoolSnapshotEntity.EndDate)},
                {nameof(LiquidityPoolSnapshotEntity.Details)},
                {nameof(LiquidityPoolSnapshotEntity.ModifiedDate)}
              ) VALUES (
                @{nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)},
                @{nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)},
                @{nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                @{nameof(LiquidityPoolSnapshotEntity.StartDate)},
                @{nameof(LiquidityPoolSnapshotEntity.EndDate)},
                @{nameof(LiquidityPoolSnapshotEntity.Details)},
                UTC_TIMESTAMP()
              );".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE pool_liquidity_snapshot
                SET
                    {nameof(LiquidityPoolSnapshotEntity.TransactionCount)} = @{nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                    {nameof(LiquidityPoolSnapshotEntity.Details)} = @{nameof(LiquidityPoolSnapshotEntity.Details)},
                    {nameof(LiquidityPoolSnapshotEntity.ModifiedDate)} = UTC_TIMESTAMP()
                WHERE {nameof(LiquidityPoolSnapshotEntity.Id)} = @{nameof(LiquidityPoolSnapshotEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistLiquidityPoolSnapshotCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistLiquidityPoolSnapshotCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistLiquidityPoolSnapshotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<LiquidityPoolSnapshotEntity>(request.Snapshot);

            var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            return await _context.ExecuteCommandAsync(command) >= 1;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "LiquidityPoolId", request.Snapshot.LiquidityPoolId },
                { "StartDate", request.Snapshot.StartDate },
                { "EndDate", request.Snapshot.EndDate },
                { "ModifiedDate", request.Snapshot.ModifiedDate }
            }))
            {
                _logger.LogError(ex, $"Failure persisting liquidity pool snapshot");
            }

            return false;
        }
    }
}