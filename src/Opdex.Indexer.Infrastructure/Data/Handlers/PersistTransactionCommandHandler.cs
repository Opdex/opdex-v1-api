using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistTransactionCommandHandler : IRequestHandler<PersistTransactionCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction (
                {nameof(TransactionEntity.From)},
                {nameof(TransactionEntity.To)},
                {nameof(TransactionEntity.TxHash)},
                {nameof(TransactionEntity.GasUsed)},
                {nameof(TransactionEntity.Block)}
              ) VALUES (
                @{nameof(TransactionEntity.From)},
                @{nameof(TransactionEntity.To)},
                @{nameof(TransactionEntity.TxHash)},
                @{nameof(TransactionEntity.GasUsed)},
                @{nameof(TransactionEntity.Block)}
              );";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public PersistTransactionCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(PersistTransactionCommand request, CancellationToken cancellationToken)
        {
            var transactionEntity = _mapper.Map<TransactionEntity>(request.Transaction);
            
            var command = DatabaseQuery.Create(SqlCommand, transactionEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);

            return result > 0;
        }
    }
}