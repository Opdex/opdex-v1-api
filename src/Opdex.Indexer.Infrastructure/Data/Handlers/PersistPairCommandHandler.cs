using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistPairCommandHandler : IRequestHandler<PersistPairCommand, bool>
    {
        // Todo: Insert vs update
        private static readonly string SqlCommand =
            $@"Insert into pair (
                {nameof(PairEntity.Address)},
                {nameof(PairEntity.TokenId)},
                {nameof(PairEntity.ReserveToken)},
                {nameof(PairEntity.ReserveCrs)}
              ) VALUES (
                @{nameof(PairEntity.Address)},
                @{nameof(PairEntity.TokenId)},
                @{nameof(PairEntity.ReserveToken)},
                @{nameof(PairEntity.ReserveCrs)}
              );";

        private readonly IDbContext _context;

        public PersistPairCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistPairCommand request, CancellationToken cancellationToken)
        {
            // Todo: Create new mapper profile or QueryParams object. Map request to entity to persist
            var command = DatabaseQuery.Create(SqlCommand, request.Pair, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);

            return result > 0;
        }
    }
}