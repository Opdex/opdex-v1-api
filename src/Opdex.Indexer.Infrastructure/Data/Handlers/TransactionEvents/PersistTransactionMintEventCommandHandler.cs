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
    public class PersistTransactionMintEventCommandHandler: IRequestHandler<PersistTransactionMintEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_mint (
                {nameof(MintEventEntity.TransactionId)},
                {nameof(MintEventEntity.Address)},
                {nameof(MintEventEntity.Sender)},
                {nameof(MintEventEntity.AmountCrs)},
                {nameof(MintEventEntity.AmountSrc)}
              ) VALUES (
                @{nameof(MintEventEntity.TransactionId)},
                @{nameof(MintEventEntity.Address)},
                @{nameof(MintEventEntity.Sender)},
                @{nameof(MintEventEntity.AmountCrs)},
                @{nameof(MintEventEntity.AmountSrc)}
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionMintEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<bool> Handle(PersistTransactionMintEventCommand request, CancellationToken cancellationToken)
        {
            var mintEventEntity = _mapper.Map<TransferEventEntity>(request.MintEvent);
            
            var command = DatabaseQuery.Create(SqlCommand, mintEventEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}