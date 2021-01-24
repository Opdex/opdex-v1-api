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
    public class PersistTransactionPairCreatedEventCommandHandler : IRequestHandler<PersistTransactionPairCreatedEventCommand>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_pair_created (
                {nameof(PairCreatedEventEntity.Id)},
                {nameof(PairCreatedEventEntity.TransactionId)}
              ) VALUES (
                @{nameof(PairCreatedEventEntity.Id)},
                @{nameof(PairCreatedEventEntity.TransactionId)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionPairCreatedEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public Task<Unit> Handle(PersistTransactionPairCreatedEventCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}