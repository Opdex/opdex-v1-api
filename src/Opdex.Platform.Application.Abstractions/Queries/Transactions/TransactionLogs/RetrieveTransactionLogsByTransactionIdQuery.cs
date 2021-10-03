using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs
{
    public class RetrieveTransactionLogsByTransactionIdQuery : IRequest<IEnumerable<TransactionLog>>
    {
        public RetrieveTransactionLogsByTransactionIdQuery(ulong transactionId)
        {
            if (transactionId < 1)
            {
                throw new ArgumentNullException(nameof(transactionId));
            }

            TransactionId = transactionId;
        }

        public ulong TransactionId { get; }
    }
}
