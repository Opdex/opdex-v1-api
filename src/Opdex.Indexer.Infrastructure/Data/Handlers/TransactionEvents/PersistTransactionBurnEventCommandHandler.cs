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
    public class PersistTransactionBurnEventCommandHandler: IRequestHandler<PersistTransactionBurnEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_burn (
                {nameof(BurnEventEntity.TransactionId)},
                {nameof(BurnEventEntity.Address)},
                {nameof(BurnEventEntity.Sender)},
                {nameof(BurnEventEntity.To)},
                {nameof(BurnEventEntity.AmountCrs)},
                {nameof(BurnEventEntity.AmountSrc)}
              ) VALUES (
                @{nameof(BurnEventEntity.TransactionId)},
                @{nameof(BurnEventEntity.Address)},
                @{nameof(BurnEventEntity.Sender)},
                @{nameof(BurnEventEntity.To)},
                @{nameof(BurnEventEntity.AmountCrs)},
                @{nameof(BurnEventEntity.AmountSrc)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionBurnEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionBurnEventCommand request, CancellationToken cancellationToken)
        {
            var burnEventEntity = _mapper.Map<TransferEventEntity>(request.BurnEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, burnEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}