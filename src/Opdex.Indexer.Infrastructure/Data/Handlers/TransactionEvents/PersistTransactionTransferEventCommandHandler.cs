using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionTransferEventCommandHandler : IRequestHandler<PersistTransactionTransferEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_transfer (
                `{nameof(TransferEventEntity.From)}`,
                `{nameof(TransferEventEntity.To)}`,
                {nameof(TransferEventEntity.Amount)},
                {nameof(TransferEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(TransferEventEntity.From)},
                @{nameof(TransferEventEntity.To)},
                @{nameof(TransferEventEntity.Amount)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionTransferEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionTransferEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionTransferEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transferEventEntity = _mapper.Map<TransferEventEntity>(request.TransferEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, transferEventEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.TransferEvent}");
                return 0;
            }
        }
    }
}