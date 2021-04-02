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
    public class SelectSwapEventsByIdsQueryHandler : IRequestHandler<SelectSwapEventsByIdsQuery, IEnumerable<SwapEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(SwapEventEntity.Id)},
                {nameof(SwapEventEntity.Sender)},
                `{nameof(SwapEventEntity.To)}`,
                {nameof(SwapEventEntity.AmountCrsIn)},
                {nameof(SwapEventEntity.AmountCrsOut)},
                {nameof(SwapEventEntity.AmountSrcIn)},
                {nameof(SwapEventEntity.AmountSrcOut)}
            FROM transaction_event_swap
            WHERE {nameof(SwapEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectSwapEventsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SwapEvent>> Handle(SelectSwapEventsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<SwapEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<SwapEvent>();

            var response = new List<SwapEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new SwapEvent(result.Id, txEvent.TransactionId, txEvent.Contract, txEvent.SortOrder, result.Sender,
                    result.To, result.AmountCrsIn, result.AmountCrsOut, result.AmountSrcIn, result.AmountSrcOut));
            }

            return !response.Any() ? Enumerable.Empty<SwapEvent>() : response;
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