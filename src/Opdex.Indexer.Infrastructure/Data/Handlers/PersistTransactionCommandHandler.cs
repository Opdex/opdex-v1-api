using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistTransactionCommandHandler : IRequestHandler<PersistTransactionCommand, long>
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
              );
              SELECT last_insert_rowid();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transactionEntity = _mapper.Map<TransactionEntity>(request.Transaction);

                var command = DatabaseQuery.Create(SqlCommand, transactionEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return result;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {nameof(request.Transaction)}");
                return 0;
            }
        }
    }
}