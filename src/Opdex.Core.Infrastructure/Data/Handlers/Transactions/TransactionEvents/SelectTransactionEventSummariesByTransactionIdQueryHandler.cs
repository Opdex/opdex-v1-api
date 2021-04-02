using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectTransactionEventSummariesByTransactionIdQueryHandler
        : IRequestHandler<SelectTransactionEventSummariesByTransactionIdQuery, IEnumerable<TransactionEventSummary>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(TransactionEventSummaryEntity.Id)},
                {nameof(TransactionEventSummaryEntity.TransactionId)},
                {nameof(TransactionEventSummaryEntity.EventId)},
                {nameof(TransactionEventSummaryEntity.EventTypeId)},
                {nameof(TransactionEventSummaryEntity.SortOrder)},
                {nameof(TransactionEventSummaryEntity.Contract)}
            FROM transaction_event_summary
            WHERE {nameof(TransactionEventSummaryEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransactionEventSummariesByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TransactionEventSummary>> Handle(SelectTransactionEventSummariesByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<TransactionEventSummaryEntity>(query);

            return !result.Any() ? Enumerable.Empty<TransactionEventSummary>() : _mapper.Map<IEnumerable<TransactionEventSummary>>(result);
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