using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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
                {nameof(PairCreatedEventEntity.Pair)}
              ) VALUES (
                @{nameof(PairCreatedEventEntity.TransactionId)},
                @{nameof(PairCreatedEventEntity.Address)},
                @{nameof(PairCreatedEventEntity.Token)},
                @{nameof(PairCreatedEventEntity.Pair)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionPairCreatedEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionPairCreatedEventCommand request, CancellationToken cancellationToken)
        {
            var pairCreatedEventEntity = _mapper.Map<TransferEventEntity>(request.PairCreatedEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, pairCreatedEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}