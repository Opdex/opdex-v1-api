using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionEventSummaryCommandHandler : IRequestHandler<PersistTransactionEventSummaryCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_summary (
                {nameof(TransactionEventSummaryEntity.TransactionId)},
                {nameof(TransactionEventSummaryEntity.EventTypeId)},
                {nameof(TransactionEventSummaryEntity.EventId)},
                {nameof(TransactionEventSummaryEntity.Contract)},
                {nameof(TransactionEventSummaryEntity.SortOrder)},
                {nameof(TransactionEventSummaryEntity.CreatedDate)}
              ) VALUES (
                @{nameof(TransactionEventSummaryEntity.TransactionId)},
                @{nameof(TransactionEventSummaryEntity.EventTypeId)},
                @{nameof(TransactionEventSummaryEntity.EventId)},
                @{nameof(TransactionEventSummaryEntity.Contract)},
                @{nameof(TransactionEventSummaryEntity.SortOrder)},
                UTC_TIMESTAMP()
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionEventSummaryCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionEventSummaryCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(PersistTransactionEventSummaryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approvalEventEntity = _mapper.Map<TransactionEventSummaryEntity>(request);

                var command = DatabaseQuery.Create(SqlCommand, approvalEventEntity, cancellationToken);

                var result = await _context.ExecuteCommandAsync(command);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {nameof(TransactionEventSummaryEntity)}");
                return false;
            }
        }
    }
}