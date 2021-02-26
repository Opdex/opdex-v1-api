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
    public class PersistTransactionApprovalEventCommandHandler: IRequestHandler<PersistTransactionApprovalEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_approval (
                {nameof(ApprovalEventEntity.TransactionId)},
                {nameof(ApprovalEventEntity.Address)},
                {nameof(ApprovalEventEntity.Owner)},
                {nameof(ApprovalEventEntity.Spender)},
                {nameof(ApprovalEventEntity.Amount)},
                {nameof(ApprovalEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(ApprovalEventEntity.TransactionId)}
                @{nameof(ApprovalEventEntity.Address)}
                @{nameof(ApprovalEventEntity.Owner)},
                @{nameof(ApprovalEventEntity.Spender)},
                @{nameof(ApprovalEventEntity.Amount)},
                UTC_TIMESTAMP()
              );";
        
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
        
        public async Task<bool> Handle(PersistTransactionApprovalEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approvalEventEntity = _mapper.Map<ApprovalEventEntity>(request.ApprovalEvent);

                var command = DatabaseQuery.Create(SqlCommand, approvalEventEntity, cancellationToken);

                var result = await _context.ExecuteCommandAsync(command);

                return result > 0;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.ApprovalEvent}");
                return false;
            }
        }
    }
}