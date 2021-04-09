using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs
{
    public class PersistTransactionTransferLogCommandHandler : IRequestHandler<PersistTransactionTransferLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_transfer (
                `{nameof(TransferLogEntity.From)}`,
                `{nameof(TransferLogEntity.To)}`,
                {nameof(TransferLogEntity.Amount)},
                {nameof(TransferLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(TransferLogEntity.From)},
                @{nameof(TransferLogEntity.To)},
                @{nameof(TransferLogEntity.Amount)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionTransferLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionTransferLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionTransferLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transferLogEntity = _mapper.Map<TransferLogEntity>(request.TransferLog);
            
                var command = DatabaseQuery.Create(SqlCommand, transferLogEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.TransferLog}");
                return 0;
            }
        }
    }
}