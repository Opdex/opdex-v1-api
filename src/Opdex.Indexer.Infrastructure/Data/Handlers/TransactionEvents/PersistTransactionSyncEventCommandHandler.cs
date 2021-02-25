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
    public class PersistTransactionSyncEventCommandHandler: IRequestHandler<PersistTransactionSyncEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_sync (
                {nameof(SyncEventEntity.TransactionId)},
                {nameof(SyncEventEntity.Address)},
                {nameof(SyncEventEntity.ReserveCrs)},
                {nameof(SyncEventEntity.ReserveSrc)}
              ) VALUES (
                @{nameof(SyncEventEntity.TransactionId)},
                @{nameof(SyncEventEntity.Address)},
                @{nameof(SyncEventEntity.ReserveCrs)},
                @{nameof(SyncEventEntity.ReserveSrc)}
              );";
        
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
        
        public async Task<bool> Handle(PersistTransactionSyncEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var syncEventEntity = _mapper.Map<SyncEventEntity>(request.SyncEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, syncEventEntity, cancellationToken);
            
                var result = await _context.ExecuteCommandAsync(command);
            
                return result > 0;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.SyncEvent}");
                return false;
            }
        }
    }
}