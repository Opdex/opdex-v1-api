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
    public class PersistTransactionApprovalEventCommandHandler: IRequestHandler<PersistTransactionApprovalEventCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_approval (
                {nameof(ApprovalEventEntity.Owner)},
                {nameof(ApprovalEventEntity.Spender)},
                {nameof(ApprovalEventEntity.Amount)},
                {nameof(ApprovalEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(ApprovalEventEntity.Owner)},
                @{nameof(ApprovalEventEntity.Spender)},
                @{nameof(ApprovalEventEntity.Amount)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionApprovalEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionApprovalEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionApprovalEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approvalEventEntity = _mapper.Map<ApprovalEventEntity>(request.ApprovalEvent);

                var command = DatabaseQuery.Create(SqlCommand, approvalEventEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.ApprovalEvent}");
                return 0;
            }
        }
    }
}