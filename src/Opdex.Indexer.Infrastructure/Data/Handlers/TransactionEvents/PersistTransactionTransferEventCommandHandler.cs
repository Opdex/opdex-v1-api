using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionTransferEventCommandHandler : IRequestHandler<PersistTransactionTransferEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_transfer (
                {nameof(TransferEventEntity.TransactionId)},
                {nameof(TransferEventEntity.Address)},
                {nameof(TransferEventEntity.From)},
                {nameof(TransferEventEntity.To)},
                {nameof(TransferEventEntity.Amount)}
              ) VALUES (
                @{nameof(TransferEventEntity.TransactionId)},
                @{nameof(TransferEventEntity.Address)},
                @{nameof(TransferEventEntity.From)},
                @{nameof(TransferEventEntity.To)},
                @{nameof(TransferEventEntity.Amount)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionTransferEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionTransferEventCommand request, CancellationToken cancellationToken)
        {
            var transferEventEntity = _mapper.Map<TransferEventEntity>(request.TransferEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, transferEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}