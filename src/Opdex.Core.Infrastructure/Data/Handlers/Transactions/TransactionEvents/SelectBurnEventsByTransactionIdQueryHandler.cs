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
    public class SelectBurnEventsByTransactionIdQueryHandler : IRequestHandler<SelectBurnEventsByTransactionIdQuery, IEnumerable<BurnEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(BurnEventEntity.Id)},
                {nameof(BurnEventEntity.Sender)},
                `{nameof(BurnEventEntity.To)}`,
                {nameof(BurnEventEntity.AmountCrs)},
                {nameof(BurnEventEntity.AmountSrc)}
            FROM transaction_event_burn
            WHERE {nameof(BurnEventEntity.Id)} IN @{nameof(SqlParams.TransactionEventIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectBurnEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<BurnEvent>> Handle(SelectBurnEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var transactionEvents = request.TransactionEvents.ToDictionary(k => k.EventId);
            
            var queryParams = new SqlParams(transactionEvents.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<BurnEventEntity>(query);

            if (!results.Any()) return Enumerable.Empty<BurnEvent>();

            var response = new List<BurnEvent>();

            foreach (var result in results)
            {
                var found = transactionEvents.TryGetValue(result.Id, out var txEvent);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new BurnEvent(result.Id, txEvent.TransactionId, txEvent.Contract, 
                    txEvent.SortOrder, result.Sender, result.To, result.AmountCrs, result.AmountSrc));
            }

            return !response.Any() ? Enumerable.Empty<BurnEvent>() : response;
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