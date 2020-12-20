using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions;
using Opdex.Core.Infrastructure.Abstractions.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Commands;

namespace Opdex.Indexer.Infrastructure.CommandHandlers
{
    public class InsertTokenCommandHandler : IRequestHandler<PersistTokenCommand>
    {
        private static readonly string InsertTokenSql =
            $@"Insert into token(
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.MaxSupply)}
              ) VALUES(
                @{nameof(TokenEntity.Address)},
                @{nameof(TokenEntity.Name)},
                @{nameof(TokenEntity.Symbol)},
                @{nameof(TokenEntity.Decimals)},
                @{nameof(TokenEntity.Sats)},
                @{nameof(TokenEntity.MaxSupply)}
              );";

        private readonly IDbContext _context;

        public InsertTokenCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(PersistTokenCommand request, CancellationToken cancellationToken)
        {
            // Todo: Create new mapper profile or QueryParams object. Map request to entity to persist
            var command = DatabaseQuery.Create(InsertTokenSql, request.Token, cancellationToken);
            
            await _context.ExecuteScalarAsync<long>(command);
            
            return Unit.Value;
        }
    }
}