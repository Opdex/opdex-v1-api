using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions;

/// <summary>
/// Checks the mempool for whether or not a transaction exists.
/// </summary>
public class RetrieveCirrusExistsInMempoolQuery : IRequest<bool>
{
    /// <summary>
    /// Creates a request to check the mempool for whether or not a transaction exists.
    /// </summary>
    /// <param name="transactionHash">Hash of the transaction.</param>
    public RetrieveCirrusExistsInMempoolQuery(Sha256 transactionHash)
    {
        TransactionHash = transactionHash;
    }

    public Sha256 TransactionHash { get; }
}
