using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks
{
    public class PersistIndexerUnlockCommandHandler : IRequestHandler<PersistIndexerUnlockCommand, Unit>
    {
        private static readonly string SqlQuery = "INSERT INTO index_lock VALUES (0, SYSDATE());";
        private readonly IDbContext _context;

        public PersistIndexerUnlockCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(PersistIndexerUnlockCommand request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlQuery, token: cancellationToken);
            await _context.ExecuteCommandAsync(command);
            return Unit.Value;
        }
    }
}