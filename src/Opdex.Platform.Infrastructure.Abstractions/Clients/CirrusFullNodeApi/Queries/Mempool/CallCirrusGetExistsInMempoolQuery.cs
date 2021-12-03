using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;

/// <summary>Query to determine whether a transaction exists in the full node mempool.</summary>
/// <remarks>The mempool stores transactions which have not yet been confirmed by the network.</remarks>
public class CallCirrusGetExistsInMempoolQuery : IRequest<bool>
{
    /// <summary>
    /// Creates a query to determine whether a transaction exists in the full node mempool.
    /// </summary>
    /// <param name="transactionHash">The hash of the transaction.</param>
    public CallCirrusGetExistsInMempoolQuery(Sha256 transactionHash)
    {
        TransactionHash = transactionHash;
    }

    public Sha256 TransactionHash { get; }
}