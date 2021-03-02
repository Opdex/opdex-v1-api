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
                {nameof(PairCreatedEventEntity.TransactionId)},
                {nameof(PairCreatedEventEntity.Address)},
                {nameof(PairCreatedEventEntity.SortOrder)},
                {nameof(PairCreatedEventEntity.Token)},
                {nameof(PairCreatedEventEntity.Pair)}
            FROM transaction_event_pair_created
            WHERE {nameof(PairCreatedEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectPairCreatedEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PairCreatedEvent>> Handle(SelectPairCreatedEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<PairCreatedEventEntity>(query);

            return !result.Any() ? Enumerable.Empty<PairCreatedEvent>() : _mapper.Map<IEnumerable<PairCreatedEvent>>(result);
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