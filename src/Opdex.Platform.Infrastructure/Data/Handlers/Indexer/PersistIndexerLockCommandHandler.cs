using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Indexer;

public class PersistIndexerLockCommandHandler : IRequestHandler<PersistIndexerLockCommand, bool>
{
    private static readonly string LockSqlQuery =
        @$"UPDATE index_lock
                SET
                    {nameof(IndexLockEntity.Locked)} = 1,
                    {nameof(IndexLockEntity.ModifiedDate)} = UTC_TIMESTAMP(),
                    {nameof(IndexLockEntity.InstanceId)} = @{nameof(SqlParams.InstanceId)},
                    {nameof(IndexLockEntity.Reason)} = @{nameof(SqlParams.Reason)}
                WHERE {nameof(IndexLockEntity.Locked)} = 0;";

    private static readonly string UpdateReasonSqlQuery =
        @$"UPDATE index_lock
                SET
                    {nameof(IndexLockEntity.Reason)} = @{nameof(SqlParams.Reason)};";

    private readonly IDbContext _context;
    private readonly ILogger<PersistIndexerLockCommandHandler> _logger;
    private readonly string _instanceId;

    public PersistIndexerLockCommandHandler(IDbContext context, ILogger<PersistIndexerLockCommandHandler> logger, OpdexConfiguration opdexConfiguration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _instanceId = opdexConfiguration?.InstanceId ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> Handle(PersistIndexerLockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var isNewLock = request.Reason is IndexLockReason.Deploying or IndexLockReason.Indexing or IndexLockReason.Searching;
            var command = DatabaseQuery.Create(isNewLock ? LockSqlQuery : UpdateReasonSqlQuery, new SqlParams(_instanceId, request.Reason), CancellationToken.None);
            var result = await _context.ExecuteCommandAsync(command);
            return result == 1;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "LockReason", request.Reason }
            }))
            {
                _logger.LogError(ex, "Failed to persist indexer lock.");
            }
            return false;
        }
    }

    private sealed class SqlParams
    {
        internal SqlParams(string instanceId, IndexLockReason reason)
        {
            InstanceId = instanceId;
            Reason = reason;
        }

        public string InstanceId { get; }
        public IndexLockReason Reason { get; }
    }
}
