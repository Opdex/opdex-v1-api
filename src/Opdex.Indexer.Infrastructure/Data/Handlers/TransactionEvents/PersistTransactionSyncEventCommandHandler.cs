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
    public class PersistTransactionSyncEventCommandHandler: IRequestHandler<PersistTransactionSyncEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_sync (
                {nameof(SyncEventEntity.ReserveCrs)},
                {nameof(SyncEventEntity.ReserveSrc)},
                {nameof(SyncEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(SyncEventEntity.ReserveCrs)},
                @{nameof(SyncEventEntity.ReserveSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionSyncEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionSyncEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionSyncEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var syncEventEntity = _mapper.Map<SyncEventEntity>(request.SyncEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, syncEventEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.SyncEvent}");
                return 0;
            }
        }
    }
}