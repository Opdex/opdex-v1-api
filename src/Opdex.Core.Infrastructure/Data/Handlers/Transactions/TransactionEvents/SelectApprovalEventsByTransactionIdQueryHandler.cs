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
    public class SelectApprovalEventsByTransactionIdQueryHandler : IRequestHandler<SelectApprovalEventsByTransactionIdQuery, IEnumerable<ApprovalEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(ApprovalEventEntity.Id)},
                {nameof(ApprovalEventEntity.Owner)},
                {nameof(ApprovalEventEntity.Spender)},
                {nameof(ApprovalEventEntity.Amount)}
            FROM transaction_event_approval
            WHERE {nameof(ApprovalEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectApprovalEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ApprovalEvent>> Handle(SelectApprovalEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<ApprovalEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<ApprovalEvent>();

            var response = new List<ApprovalEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new ApprovalEvent(result.Id, txEvent.TransactionId, txEvent.Contract, 
                    txEvent.SortOrder, result.Owner, result.Spender, result.Amount));
            }

            return !response.Any() ? Enumerable.Empty<ApprovalEvent>() : response;
        }

        private sealed class SqlParams
        {
            internal SqlParams(IEnumerable<long> transactionEventIds)
            {
                TransactionEventIds = transactionEventIds;
            }

            public IEnumerable<long> TransactionEventIds { get; }
        }
    }
}