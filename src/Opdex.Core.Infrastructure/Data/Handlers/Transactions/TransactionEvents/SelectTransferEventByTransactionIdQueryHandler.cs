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
    public class SelectTransferEventByTransactionIdQueryHandler : IRequestHandler<SelectTransferEventByTransactionIdQuery, TransferEvent>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(TransferEventEntity.Id)},
                {nameof(TransferEventEntity.TransactionId)},
                {nameof(TransferEventEntity.Address)},
                {nameof(TransferEventEntity.SortOrder)},
                {nameof(TransferEventEntity.To)},
                {nameof(TransferEventEntity.From)},
                {nameof(TransferEventEntity.Amount)}
            FROM transaction_event_transfer
            WHERE {nameof(TransferEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransferEventByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransferEvent> Handle(SelectTransferEventByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteFindAsync<TransferEventEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(TransferEventEntity)} with transactionId {request.TransactionId} was not found.");
            }

            return _mapper.Map<TransferEvent>(result);
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