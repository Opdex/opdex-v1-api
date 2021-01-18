using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistBlockCommandHandler : IRequestHandler<PersistBlockCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO block (
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.Time)},
                {nameof(BlockEntity.MedianTime)}
              ) VALUES (
                @{nameof(BlockEntity.Height)},
                @{nameof(BlockEntity.Hash)},
                @{nameof(BlockEntity.Time)},
                @{nameof(BlockEntity.MedianTime)}
              );";

        private readonly IDbContext _context;

        public PersistBlockCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistBlockCommand request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, request.Block, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}