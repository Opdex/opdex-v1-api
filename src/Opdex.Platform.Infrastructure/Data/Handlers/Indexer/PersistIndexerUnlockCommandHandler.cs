using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Indexer
{
    public class PersistIndexerUnlockCommandHandler : IRequestHandler<PersistIndexerUnlockCommand, bool>
    {
        private static readonly string SqlQuery =
            $@"UPDATE index_lock SET
                {nameof(IndexLockEntity.Locked)} = 0,
                {nameof(IndexLockEntity.ModifiedDate)} = UTC_TIMESTAMP();";

        private readonly IDbContext _context;

        public PersistIndexerUnlockCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistIndexerUnlockCommand request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlQuery);
            var result = await _context.ExecuteCommandAsync(command);
            return result == 1;
        }
    }
}
