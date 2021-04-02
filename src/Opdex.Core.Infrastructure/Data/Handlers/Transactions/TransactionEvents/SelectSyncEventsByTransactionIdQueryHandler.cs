using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectSyncEventsByTransactionIdQueryHandler : IRequestHandler<SelectSyncEventsByTransactionIdQuery, IEnumerable<SyncEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(SyncEventEntity.Id)},
                {nameof(SyncEventEntity.ReserveCrs)},
                {nameof(SyncEventEntity.ReserveSrc)}
            FROM transaction_event_sync
            WHERE {nameof(SyncEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectSyncEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SyncEvent>> Handle(SelectSyncEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<SyncEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<SyncEvent>();

            var response = new List<SyncEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new SyncEvent(result.Id, txEvent.TransactionId, txEvent.Contract, 
                    txEvent.SortOrder, result.ReserveCrs, result.ReserveSrc));
            }

            return !response.Any() ? Enumerable.Empty<SyncEvent>() : response;
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