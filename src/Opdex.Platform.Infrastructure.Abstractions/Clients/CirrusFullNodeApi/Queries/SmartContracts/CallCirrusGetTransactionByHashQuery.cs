using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    /// <summary>
    /// Retrieves transaction details of a certain transaction.
    /// </summary>
    public class CallCirrusGetTransactionByHashQuery : IRequest<Transaction>
    {
        /// <summary>
        /// Creates a query to retrieve the transaction details of a certain transaction.
        /// </summary>
        /// <param name="txHash">The hash of the transaction.</param>
        public CallCirrusGetTransactionByHashQuery(Sha256 txHash)
        {
            TxHash = txHash;
        }

        public Sha256 TxHash { get; }
    }
}
