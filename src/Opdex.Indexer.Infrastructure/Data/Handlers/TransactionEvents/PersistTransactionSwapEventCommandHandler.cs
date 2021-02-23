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
    public class PersistTransactionSwapEventCommandHandler: IRequestHandler<PersistTransactionSwapEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_swap (
                {nameof(SwapEventEntity.TransactionId)},
                {nameof(SwapEventEntity.Address)},
                {nameof(SwapEventEntity.Sender)},
                {nameof(SwapEventEntity.To)},
                {nameof(SwapEventEntity.AmountCrsIn)},
                {nameof(SwapEventEntity.AmountSrcIn)},
                {nameof(SwapEventEntity.AmountCrsOut)},
                {nameof(SwapEventEntity.AmountSrcOut)}
              ) VALUES (
                @{nameof(SwapEventEntity.TransactionId)},
                @{nameof(SwapEventEntity.Address)},
                @{nameof(SwapEventEntity.Sender)},
                @{nameof(SwapEventEntity.To)},
                @{nameof(SwapEventEntity.AmountCrsIn)},
                @{nameof(SwapEventEntity.AmountSrcIn)},
                @{nameof(SwapEventEntity.AmountCrsOut)},
                @{nameof(SwapEventEntity.AmountSrcOut)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionSwapEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionSwapEventCommand request, CancellationToken cancellationToken)
        {
            var swapEventEntity = _mapper.Map<TransferEventEntity>(request.SwapEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, swapEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}