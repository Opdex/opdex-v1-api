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

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots;

public class PersistMarketSnapshotCommandHandler : IRequestHandler<PersistMarketSnapshotCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market_snapshot (
                {nameof(MarketSnapshotEntity.MarketId)},
                {nameof(MarketSnapshotEntity.Details)},
                {nameof(MarketSnapshotEntity.SnapshotTypeId)},
                {nameof(MarketSnapshotEntity.StartDate)},
                {nameof(MarketSnapshotEntity.EndDate)},
                {nameof(MarketSnapshotEntity.ModifiedDate)}
              ) VALUES (
                @{nameof(MarketSnapshotEntity.MarketId)},
                @{nameof(MarketSnapshotEntity.Details)},
                @{nameof(MarketSnapshotEntity.SnapshotTypeId)},
                @{nameof(MarketSnapshotEntity.StartDate)},
                @{nameof(MarketSnapshotEntity.EndDate)},
                UTC_TIMESTAMP()
              );".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE market_snapshot
                SET
                    {nameof(MarketSnapshotEntity.Details)} = @{nameof(MarketSnapshotEntity.Details)},
                    {nameof(MarketSnapshotEntity.ModifiedDate)} = UTC_TIMESTAMP()
                WHERE {nameof(MarketSnapshotEntity.Id)} = @{nameof(MarketSnapshotEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistMarketSnapshotCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketSnapshotCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistMarketSnapshotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MarketSnapshotEntity>(request.Snapshot);

            var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            return await _context.ExecuteCommandAsync(command) >= 1;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "MarketId", request.Snapshot.MarketId },
                { "SnapshotType", request.Snapshot.SnapshotType },
                { "StartDate", request.Snapshot.StartDate },
                { "EndDate", request.Snapshot.EndDate }
            }))
            {
                _logger.LogError(ex, $"Failure persisting market snapshot.");
            }

            return false;
        }
    }
}