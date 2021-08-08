using MediatR;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Indexer
{
    public class PersistIndexerLockCommandHandler : IRequestHandler<PersistIndexerLockCommand, bool>
    {
        private static readonly string SqlQuery =
            @$"UPDATE index_lock
               SET
                    {nameof(IndexLockEntity.Locked)} = 1,
                    {nameof(IndexLockEntity.ModifiedDate)} = UTC_TIMESTAMP(),
                    {nameof(IndexLockEntity.InstanceId)} = @{nameof(IndexLockEntity.InstanceId)}
                WHERE {nameof(IndexLockEntity.Locked)} = 0;";

        private readonly IDbContext _context;
        private readonly string _instanceId;

        public PersistIndexerLockCommandHandler(IDbContext context, OpdexConfiguration opdexConfiguration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _instanceId = opdexConfiguration?.InstanceId ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistIndexerLockCommand request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlQuery, new { InstanceId = _instanceId });
            var result = await _context.ExecuteCommandAsync(command);
            return result == 1;
        }
    }
}
