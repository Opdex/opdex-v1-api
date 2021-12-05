using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Indexer;

public class PersistIndexerUnlockCommandHandler : AsyncRequestHandler<PersistIndexerUnlockCommand>
{
    private static readonly string SqlQuery =
        $@"UPDATE index_lock
            SET
                {nameof(IndexLockEntity.Locked)} = 0,
                {nameof(IndexLockEntity.ModifiedDate)} = UTC_TIMESTAMP(),
                {nameof(IndexLockEntity.InstanceId)} = NULL,
                {nameof(IndexLockEntity.Reason)} = NULL
            WHERE {nameof(IndexLockEntity.InstanceId)} = @{nameof(IndexLockEntity.InstanceId)};";

    private readonly IDbContext _context;
    private readonly ILogger<PersistIndexerUnlockCommandHandler> _logger;
    private readonly string _instanceId;

    public PersistIndexerUnlockCommandHandler(IDbContext context, ILogger<PersistIndexerUnlockCommandHandler> logger, OpdexConfiguration opdexConfiguration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _instanceId = opdexConfiguration?.InstanceId ?? throw new ArgumentNullException(nameof(context));
    }

    protected override async Task Handle(PersistIndexerUnlockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = DatabaseQuery.Create(SqlQuery, new { InstanceId = _instanceId }, CancellationToken.None);
            var result = await _context.ExecuteCommandAsync(command);
            if (result == 0) throw new NoRowsAffectedException("Indexer appears to have not unlocked.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to unlock indexer.");
        }
    }
}
