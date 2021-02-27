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
    public class SelectApprovalEventByTransactionIdQueryHandler : IRequestHandler<SelectApprovalEventByTransactionIdQuery, ApprovalEvent>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(ApprovalEventEntity.Id)},
                {nameof(ApprovalEventEntity.TransactionId)},
                {nameof(ApprovalEventEntity.Address)},
                {nameof(ApprovalEventEntity.SortOrder)},
                {nameof(ApprovalEventEntity.Owner)},
                {nameof(ApprovalEventEntity.Spender)},
                {nameof(ApprovalEventEntity.Amount)}
            FROM transaction_event_approval
            WHERE {nameof(ApprovalEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectApprovalEventByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApprovalEvent> Handle(SelectApprovalEventByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteFindAsync<ApprovalEventEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(ApprovalEventEntity)} with transactionId {request.TransactionId} was not found.");
            }

            return _mapper.Map<ApprovalEvent>(result);
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