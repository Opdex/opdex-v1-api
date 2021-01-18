using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistTokenCommandHandler : IRequestHandler<PersistTokenCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"Insert into token (
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.MaxSupply)}
              ) VALUES (
                @{nameof(TokenEntity.Address)},
                @{nameof(TokenEntity.Name)},
                @{nameof(TokenEntity.Symbol)},
                @{nameof(TokenEntity.Decimals)},
                @{nameof(TokenEntity.Sats)},
                @{nameof(TokenEntity.MaxSupply)}
              );";

        private readonly IDbContext _context;

        public PersistTokenCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistTokenCommand request, CancellationToken cancellationToken)
        {
            // Todo: Create new mapper profile or QueryParams object. Map request to entity to persist
            var command = DatabaseQuery.Create(SqlCommand, request.Token, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);

            return result > 0;
        }
    }
}