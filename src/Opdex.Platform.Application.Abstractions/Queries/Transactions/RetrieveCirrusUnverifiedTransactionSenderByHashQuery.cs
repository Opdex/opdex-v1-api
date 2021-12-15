using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions;

/// <summary>
/// Retrieves the sender of a transaction that is either in the mempool or broadcast.
/// </summary>
public class RetrieveCirrusUnverifiedTransactionSenderByHashQuery : IRequest<Address>
{

    /// <summary>
    /// Creates a request to retrieve the sender of a transaction that is either in the mempool or broadcast.
    /// </summary>
    /// <param name="transactionHash">Hash of the transaction.</param>
    public RetrieveCirrusUnverifiedTransactionSenderByHashQuery(Sha256 transactionHash)
    {
        TransactionHash = transactionHash;
    }

    public Sha256 TransactionHash { get; }
}
