using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

public class SelectTransactionLogsByTransactionIdQuery : IRequest<IEnumerable<TransactionLog>>
{
    public SelectTransactionLogsByTransactionIdQuery(ulong transactionId)
    {
        if (transactionId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(transactionId));
        }

        TransactionId = transactionId;
    }

    public ulong TransactionId { get; }
}