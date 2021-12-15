using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;

/// <summary>Query the retrieve a raw transaction from the node.</summary>
/// <remarks>Transactions get looked up from the mempool and from those which have been broadcast.</remarks>
public class CallCirrusGetRawTransactionQuery : IRequest<RawTransactionDto>
{
    /// <summary>
    /// Creates a query to retrieve the raw transaction from the node.
    /// </summary>
    /// <param name="transactionHash">The hash of the transaction.</param>
    public CallCirrusGetRawTransactionQuery(Sha256 transactionHash)
    {
        TransactionHash = transactionHash;
    }

    public Sha256 TransactionHash { get; }
}
