using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionLogs
{
    public class SelectApprovalLogsByIdsQueryHandler : IRequestHandler<SelectApprovalLogsByIdsQuery, IEnumerable<ApprovalLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(ApprovalLogEntity.Id)},
                {nameof(ApprovalLogEntity.Owner)},
                {nameof(ApprovalLogEntity.Spender)},
                {nameof(ApprovalLogEntity.Amount)}
            FROM transaction_log_approval
            WHERE {nameof(ApprovalLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectApprovalLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ApprovalLog>> Handle(SelectApprovalLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<ApprovalLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<ApprovalLog>();

            var response = new List<ApprovalLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new ApprovalLog(result.Id, txLog.TransactionId, txLog.Contract, 
                    txLog.SortOrder, result.Owner, result.Spender, result.Amount));
            }

            return !response.Any() ? Enumerable.Empty<ApprovalLog>() : response;
        }

        private sealed class SqlParams
        {
            internal SqlParams(IEnumerable<long> transactionLogIds)
            {
                TransactionLogIds = transactionLogIds;
            }

            public IEnumerable<long> TransactionLogIds { get; }
        }
    }
}