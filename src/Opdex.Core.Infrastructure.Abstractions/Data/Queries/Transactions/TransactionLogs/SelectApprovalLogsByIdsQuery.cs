using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs
{
    public class SelectApprovalLogsByIdsQuery : IRequest<IEnumerable<ApprovalLog>>
    {
        public SelectApprovalLogsByIdsQuery(IEnumerable<TransactionLogSummary> txLogs)
        {
            var logIds = txLogs as TransactionLogSummary[] ?? txLogs.ToArray();
            
            if (!logIds.Any() || logIds.Any(t => t.LogId < 1))
            {
                throw new ArgumentOutOfRangeException(nameof(txLogs));
            }

            TransactionLogs = logIds;
        }
        
        public IEnumerable<TransactionLogSummary> TransactionLogs { get; }
    }
}