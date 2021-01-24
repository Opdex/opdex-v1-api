using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistTransactionCommandHandler : IRequestHandler<PersistTransactionCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"Insert into transaction (
                {nameof(TransactionEntity.Id)},
                {nameof(TransactionEntity.From)},
                {nameof(TransactionEntity.To)},
                {nameof(TransactionEntity.TxHash)},
                {nameof(TransactionEntity.GasUsed)},
                {nameof(TransactionEntity.Block)}
              ) VALUES (
                @{nameof(TransactionEntity.Id)},
                @{nameof(TransactionEntity.From)},
                @{nameof(TransactionEntity.To)},
                @{nameof(TransactionEntity.TxHash)},
                @{nameof(TransactionEntity.GasUsed)},
                @{nameof(TransactionEntity.Block)}
              );";

        private readonly IDbContext _context;

        public PersistTransactionCommandHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(PersistTransactionCommand request, CancellationToken cancellationToken)
        {
            // Todo: Create new mapper profile or QueryParams object. Map request to entity to persist
            var command = DatabaseQuery.Create(SqlCommand, request.Transaction, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);

            return result > 0;
        }
    }
}