using System;
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
    public class SelectBurnEventByTransactionIdQueryHandler : IRequestHandler<SelectBurnEventByTransactionIdQuery, BurnEvent>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(BurnEventEntity.Id)},
                {nameof(BurnEventEntity.TransactionId)},
                {nameof(BurnEventEntity.Address)},
                {nameof(BurnEventEntity.SortOrder)},
                {nameof(BurnEventEntity.Sender)},
                {nameof(BurnEventEntity.To)},
                {nameof(BurnEventEntity.AmountCrs)},
                {nameof(BurnEventEntity.AmountSrc)}
            FROM transaction_event_burn
            WHERE {nameof(BurnEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectBurnEventByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BurnEvent> Handle(SelectBurnEventByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteFindAsync<BurnEventEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(BurnEventEntity)} with transactionId {request.TransactionId} was not found.");
            }

            return _mapper.Map<BurnEvent>(result);
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