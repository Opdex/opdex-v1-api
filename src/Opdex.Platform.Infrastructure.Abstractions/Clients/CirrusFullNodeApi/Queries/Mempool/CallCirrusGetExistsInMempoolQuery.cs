using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool
{
    /// <summary>Query to determine whether a transaction exists in the full node mempool.</summary>
    /// <remarks>The mempool stores transactions which have not yet been confirmed by the network.</remarks>
    public class CallCirrusGetExistsInMempoolQuery : IRequest<bool>
    {
        public CallCirrusGetExistsInMempoolQuery(string transactionHash)
        {
            TransactionHash = transactionHash.HasValue() ? transactionHash : throw new ArgumentNullException(nameof(transactionHash), "Transaction hash must be set.");
        }

        public string TransactionHash { get; }
    }
}
