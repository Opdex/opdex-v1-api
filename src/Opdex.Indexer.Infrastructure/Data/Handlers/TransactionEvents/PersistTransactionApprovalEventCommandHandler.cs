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
    public class PersistTransactionApprovalEventCommandHandler: IRequestHandler<PersistTransactionApprovalEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_approval (
                {nameof(ApprovalEventEntity.TransactionId)},
                {nameof(ApprovalEventEntity.Address)},
                {nameof(ApprovalEventEntity.Owner)},
                {nameof(ApprovalEventEntity.Spender)},
                {nameof(ApprovalEventEntity.Amount)}
              ) VALUES (
                @{nameof(ApprovalEventEntity.TransactionId)}
                @{nameof(ApprovalEventEntity.Address)}
                @{nameof(ApprovalEventEntity.Owner)},
                @{nameof(ApprovalEventEntity.Spender)},
                @{nameof(ApprovalEventEntity.Amount)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionApprovalEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionApprovalEventCommand request, CancellationToken cancellationToken)
        {
            var approvalEventEntity = _mapper.Map<ApprovalEventEntity>(request.ApprovalEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, approvalEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}