using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents
{
    public class SelectMintEventsByTransactionIdQuery : IRequest<IEnumerable<MintEvent>>
    {
        public SelectMintEventsByTransactionIdQuery(long transactionId)
        {
            if (transactionId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionId));
            }

            TransactionId = transactionId;
        }
        
        public long TransactionId { get; }
    }
}