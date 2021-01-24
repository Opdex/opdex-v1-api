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
    public class PersistTransactionApprovalEventCommandHandler: IRequestHandler<PersistTransactionApprovalEventCommand>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_approval (
                {nameof(ApprovalEventEntity.Id)},
                {nameof(ApprovalEventEntity.TransactionId)}
              ) VALUES (
                @{nameof(ApprovalEventEntity.Id)},
                @{nameof(ApprovalEventEntity.TransactionId)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionApprovalEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public Task<Unit> Handle(PersistTransactionApprovalEventCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}