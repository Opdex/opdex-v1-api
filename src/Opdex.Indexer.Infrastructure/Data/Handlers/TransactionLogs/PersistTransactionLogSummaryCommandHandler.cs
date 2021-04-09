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
    public class PersistTransactionLogSummaryCommandHandler : IRequestHandler<PersistTransactionLogSummaryCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_summary (
                {nameof(TransactionLogSummaryEntity.TransactionId)},
                {nameof(TransactionLogSummaryEntity.LogTypeId)},
                {nameof(TransactionLogSummaryEntity.LogId)},
                {nameof(TransactionLogSummaryEntity.Contract)},
                {nameof(TransactionLogSummaryEntity.SortOrder)},
                {nameof(TransactionLogSummaryEntity.CreatedDate)}
              ) VALUES (
                @{nameof(TransactionLogSummaryEntity.TransactionId)},
                @{nameof(TransactionLogSummaryEntity.LogTypeId)},
                @{nameof(TransactionLogSummaryEntity.LogId)},
                @{nameof(TransactionLogSummaryEntity.Contract)},
                @{nameof(TransactionLogSummaryEntity.SortOrder)},
                UTC_TIMESTAMP()
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionLogSummaryCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionLogSummaryCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(PersistTransactionLogSummaryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approvalLogEntity = _mapper.Map<TransactionLogSummaryEntity>(request);

                var command = DatabaseQuery.Create(SqlCommand, approvalLogEntity, cancellationToken);

                var result = await _context.ExecuteCommandAsync(command);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {nameof(TransactionLogSummaryEntity)}");
                return false;
            }
        }
    }
}