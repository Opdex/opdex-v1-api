using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
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
        private readonly ILogger _logger;
        
        public PersistTransactionMintEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionMintEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(PersistTransactionMintEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mintEventEntity = _mapper.Map<MintEventEntity>(request.MintEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, mintEventEntity, cancellationToken);
            
                var result = await _context.ExecuteCommandAsync(command);
            
                return result > 0;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.MintEvent}");
                return false;
            }
        }
    }
}