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
    public class SelectPoolCreatedEventsByIdsQueryHandler : IRequestHandler<SelectPoolCreatedEventsByIdsQuery, IEnumerable<PoolCreatedEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(PoolCreatedEventEntity.Id)},
                {nameof(PoolCreatedEventEntity.Token)},
                {nameof(PoolCreatedEventEntity.Pool)}
            FROM transaction_event_pool_created
            WHERE {nameof(PoolCreatedEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectPoolCreatedEventsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PoolCreatedEvent>> Handle(SelectPoolCreatedEventsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<PoolCreatedEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<PoolCreatedEvent>();

            var response = new List<PoolCreatedEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new PoolCreatedEvent(result.Id, txEvent.TransactionId, txEvent.Contract, 
                    txEvent.SortOrder, result.Token, result.Pool));
            }

            return !response.Any() ? Enumerable.Empty<PoolCreatedEvent>() : response;
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