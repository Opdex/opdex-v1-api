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
    public class PersistTransactionPoolCreatedEventCommandHandler : IRequestHandler<PersistTransactionPoolCreatedEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_pool_created (
                {nameof(PoolCreatedEventEntity.Token)},
                {nameof(PoolCreatedEventEntity.Pool)},
                {nameof(PoolCreatedEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(PoolCreatedEventEntity.Token)},
                @{nameof(PoolCreatedEventEntity.Pool)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionPoolCreatedEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionPoolCreatedEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionPoolCreatedEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolCreatedEventEntity = _mapper.Map<PoolCreatedEventEntity>(request.PoolCreatedEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, poolCreatedEventEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.PoolCreatedEvent}");
                
                return 0;
            }
        }
    }
}