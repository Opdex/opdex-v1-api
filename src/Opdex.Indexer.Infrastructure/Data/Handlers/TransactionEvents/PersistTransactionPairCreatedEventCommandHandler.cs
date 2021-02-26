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
    public class PersistTransactionPairCreatedEventCommandHandler : IRequestHandler<PersistTransactionPairCreatedEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_pair_created (
                {nameof(PairCreatedEventEntity.TransactionId)},
                {nameof(PairCreatedEventEntity.Address)},
                {nameof(PairCreatedEventEntity.Token)},
                {nameof(PairCreatedEventEntity.Pair)},
                {nameof(PairCreatedEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(PairCreatedEventEntity.TransactionId)},
                @{nameof(PairCreatedEventEntity.Address)},
                @{nameof(PairCreatedEventEntity.Token)},
                @{nameof(PairCreatedEventEntity.Pair)},
                UTC_TIMESTAMP()
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionPairCreatedEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionPairCreatedEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(PersistTransactionPairCreatedEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pairCreatedEventEntity = _mapper.Map<PairCreatedEventEntity>(request.PairCreatedEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, pairCreatedEventEntity, cancellationToken);
            
                var result = await _context.ExecuteCommandAsync(command);
            
                return result > 0;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.PairCreatedEvent}");
                return false;
            }
        }
    }
}