using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionMintEventCommandHandler: IRequestHandler<PersistTransactionMintEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_mint (
                {nameof(MintEventEntity.Sender)},
                {nameof(MintEventEntity.AmountCrs)},
                {nameof(MintEventEntity.AmountSrc)},
                {nameof(MintEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(MintEventEntity.Sender)},
                @{nameof(MintEventEntity.AmountCrs)},
                @{nameof(MintEventEntity.AmountSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
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
        
        public async Task<long> Handle(PersistTransactionMintEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mintEventEntity = _mapper.Map<MintEventEntity>(request.MintEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, mintEventEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.MintEvent}");
                return 0;
            }
        }
    }
}