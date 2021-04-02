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
    public class SelectPairCreatedEventsByTransactionIdQueryHandler : IRequestHandler<SelectPairCreatedEventsByTransactionIdQuery, IEnumerable<PairCreatedEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(PairCreatedEventEntity.Id)},
                {nameof(PairCreatedEventEntity.Token)},
                {nameof(PairCreatedEventEntity.Pair)}
            FROM transaction_event_pair_created
            WHERE {nameof(PairCreatedEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectPairCreatedEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PairCreatedEvent>> Handle(SelectPairCreatedEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<PairCreatedEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<PairCreatedEvent>();

            var response = new List<PairCreatedEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new PairCreatedEvent(result.Id, txEvent.TransactionId, txEvent.Contract, 
                    txEvent.SortOrder, result.Token, result.Pair));
            }

            return !response.Any() ? Enumerable.Empty<PairCreatedEvent>() : response;
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