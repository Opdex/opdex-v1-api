using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;

public class PersistTokenSnapshotCommandHandler : IRequestHandler<PersistTokenSnapshotCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_snapshot (
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Details)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.ModifiedDate)}
              ) VALUES (
                @{nameof(TokenSnapshotEntity.TokenId)},
                @{nameof(TokenSnapshotEntity.MarketId)},
                @{nameof(TokenSnapshotEntity.Details)},
                @{nameof(TokenSnapshotEntity.SnapshotTypeId)},
                @{nameof(TokenSnapshotEntity.StartDate)},
                @{nameof(TokenSnapshotEntity.EndDate)},
                UTC_TIMESTAMP()
              );".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE token_snapshot 
                SET 
                    {nameof(TokenSnapshotEntity.Details)} = @{nameof(TokenSnapshotEntity.Details)},
                    {nameof(TokenSnapshotEntity.ModifiedDate)} = UTC_TIMESTAMP()
                WHERE {nameof(TokenSnapshotEntity.Id)} = @{nameof(TokenSnapshotEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenSnapshotCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenSnapshotCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistTokenSnapshotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TokenSnapshotEntity>(request.Snapshot);

            var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            return await _context.ExecuteCommandAsync(command) >= 1;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "MarketId", request.Snapshot.MarketId },
                { "TokenId", request.Snapshot.TokenId },
                { "SnapshotType", request.Snapshot.SnapshotType },
                { "StartDate", request.Snapshot.StartDate },
                { "EndDate", request.Snapshot.EndDate },
                { "ModifiedDate", request.Snapshot.ModifiedDate },
            }))
            {
                _logger.LogError(ex, $"Failure persisting token snapshot.");
            }

            return false;
        }
    }
}