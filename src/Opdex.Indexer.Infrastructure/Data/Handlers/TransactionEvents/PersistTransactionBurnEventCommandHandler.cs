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
    public class PersistTransactionBurnEventCommandHandler: IRequestHandler<PersistTransactionBurnEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_burn (
                {nameof(BurnEventEntity.Sender)},
                `{nameof(BurnEventEntity.To)}`,
                {nameof(BurnEventEntity.AmountCrs)},
                {nameof(BurnEventEntity.AmountSrc)},
                {nameof(BurnEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(BurnEventEntity.Sender)},
                @{nameof(BurnEventEntity.To)},
                @{nameof(BurnEventEntity.AmountCrs)},
                @{nameof(BurnEventEntity.AmountSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionBurnEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionBurnEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionBurnEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var burnEventEntity = _mapper.Map<BurnEventEntity>(request.BurnEvent);
            
                var command = DatabaseQuery.Create(SqlCommand, burnEventEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.BurnEvent}");
                return 0;
            }
        }
    }
}