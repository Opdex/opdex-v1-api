using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionLogs
{
    public class SelectTransactionLogSummariesByTransactionIdQueryHandler
        : IRequestHandler<SelectTransactionLogSummariesByTransactionIdQuery, IEnumerable<TransactionLogSummary>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(TransactionLogSummaryEntity.Id)},
                {nameof(TransactionLogSummaryEntity.TransactionId)},
                {nameof(TransactionLogSummaryEntity.LogId)},
                {nameof(TransactionLogSummaryEntity.LogTypeId)},
                {nameof(TransactionLogSummaryEntity.SortOrder)},
                {nameof(TransactionLogSummaryEntity.Contract)}
            FROM transaction_log_summary
            WHERE {nameof(TransactionLogSummaryEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransactionLogSummariesByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TransactionLogSummary>> Handle(SelectTransactionLogSummariesByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<TransactionLogSummaryEntity>(query);

            return !result.Any() ? Enumerable.Empty<TransactionLogSummary>() : _mapper.Map<IEnumerable<TransactionLogSummary>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long transactionId)
            {
                TransactionId = transactionId;
            }

            public long TransactionId { get; }
        }
    }
}