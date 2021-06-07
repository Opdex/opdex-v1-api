using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks
{
    public class PersistIndexerLockCommandHandler : IRequestHandler<PersistIndexerLockCommand, bool>
    {
        private static readonly string SqlQuery =
            @$"UPDATE index_lock SET
                {nameof(IndexLockEntity.Locked)} = 1,
                {nameof(IndexLockEntity.ModifiedDate)} = UTC_TIMESTAMP()
                    WHERE {nameof(IndexLockEntity.Locked)} = 0";

        private readonly IDbContext _context;

        public PersistIndexerLockCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistIndexerLockCommand request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlQuery, token: cancellationToken);
            var result = await _context.ExecuteCommandAsync(command);
            return result == 1;
        }
    }
}