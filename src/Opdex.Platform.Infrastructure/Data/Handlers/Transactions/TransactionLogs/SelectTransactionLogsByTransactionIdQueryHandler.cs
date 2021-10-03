using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs
{
    public class SelectTransactionLogsByTransactionIdQueryHandler
        : IRequestHandler<SelectTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(TransactionLogEntity.Id)},
                {nameof(TransactionLogEntity.TransactionId)},
                {nameof(TransactionLogEntity.LogTypeId)},
                {nameof(TransactionLogEntity.SortOrder)},
                {nameof(TransactionLogEntity.Contract)},
                {nameof(TransactionLogEntity.Details)}
            FROM transaction_log
            WHERE {nameof(TransactionLogEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransactionLogsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TransactionLog>> Handle(SelectTransactionLogsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<TransactionLogEntity>(query);

            return !result.Any() ? Enumerable.Empty<TransactionLog>() : _mapper.Map<IEnumerable<TransactionLog>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong transactionId)
            {
                TransactionId = transactionId;
            }

            public ulong TransactionId { get; }
        }
    }
}
