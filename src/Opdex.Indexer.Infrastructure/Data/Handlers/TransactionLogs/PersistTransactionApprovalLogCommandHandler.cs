using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs
{
    public class PersistTransactionApprovalLogCommandHandler: IRequestHandler<PersistTransactionApprovalLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_approval (
                {nameof(ApprovalLogEntity.Owner)},
                {nameof(ApprovalLogEntity.Spender)},
                {nameof(ApprovalLogEntity.Amount)},
                {nameof(ApprovalLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(ApprovalLogEntity.Owner)},
                @{nameof(ApprovalLogEntity.Spender)},
                @{nameof(ApprovalLogEntity.Amount)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionApprovalLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionApprovalLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionApprovalLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approvalLogEntity = _mapper.Map<ApprovalLogEntity>(request.ApprovalLog);

                var command = DatabaseQuery.Create(SqlCommand, approvalLogEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.ApprovalLog}");
                return 0;
            }
        }
    }
}