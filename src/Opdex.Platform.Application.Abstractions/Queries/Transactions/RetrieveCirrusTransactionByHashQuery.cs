using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions;

public class RetrieveCirrusTransactionByHashQuery : IRequest<Transaction>
{
    /// <summary>
    /// Creates a query to retrieve a Cirrus transaction by its hash
    /// </summary>
    /// <remarks>Expects to find the transaction, so will retry in the event of failure</remarks>
    /// <param name="txHash">Hash of the transaction</param>
    public RetrieveCirrusTransactionByHashQuery(Sha256 txHash)
    {
        TxHash = txHash;
    }

    public Sha256 TxHash { get; }
}
