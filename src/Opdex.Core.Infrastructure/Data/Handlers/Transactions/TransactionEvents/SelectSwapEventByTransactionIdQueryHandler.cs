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
    public class SelectSwapEventByTransactionIdQueryHandler : IRequestHandler<SelectSwapEventByTransactionIdQuery, SwapEvent>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(SwapEventEntity.Id)},
                {nameof(SwapEventEntity.TransactionId)},
                {nameof(SwapEventEntity.Address)},
                {nameof(SwapEventEntity.SortOrder)},
                {nameof(SwapEventEntity.Sender)},
                {nameof(SwapEventEntity.To)},
                {nameof(SwapEventEntity.AmountCrsIn)},
                {nameof(SwapEventEntity.AmountCrsOut)},
                {nameof(SwapEventEntity.AmountSrcIn)},
                {nameof(SwapEventEntity.AmountSrcOut)}
            FROM transaction_event_swap
            WHERE {nameof(SwapEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectSwapEventByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SwapEvent> Handle(SelectSwapEventByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteFindAsync<SwapEventEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(SwapEventEntity)} with transactionId {request.TransactionId} was not found.");
            }

            return _mapper.Map<SwapEvent>(result);
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