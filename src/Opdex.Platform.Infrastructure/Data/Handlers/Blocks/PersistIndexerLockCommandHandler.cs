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
            @$"INSERT INTO index_lock
                (SELECT 1, SYSDATE() FROM index_lock
                    WHERE 0 =
                        (SELECT {nameof(IndexLockEntity.Locked)} FROM index_lock
                            WHERE {nameof(IndexLockEntity.ModifiedDate)} =
                                (SELECT Max({nameof(IndexLockEntity.ModifiedDate)}) FROM index_lock)) LIMIT 1);";

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