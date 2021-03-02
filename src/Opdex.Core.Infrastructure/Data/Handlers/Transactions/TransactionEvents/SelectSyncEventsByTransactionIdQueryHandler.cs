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
                {nameof(SyncEventEntity.TransactionId)},
                {nameof(SyncEventEntity.Address)},
                {nameof(SyncEventEntity.SortOrder)},
                {nameof(SyncEventEntity.ReserveCrs)},
                {nameof(SyncEventEntity.ReserveSrc)}
            FROM transaction_event_sync
            WHERE {nameof(SyncEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectSyncEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SyncEvent>> Handle(SelectSyncEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<SyncEventEntity>(query);

            return !result.Any() ? Enumerable.Empty<SyncEvent>() : _mapper.Map<IEnumerable<SyncEvent>>(result);
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