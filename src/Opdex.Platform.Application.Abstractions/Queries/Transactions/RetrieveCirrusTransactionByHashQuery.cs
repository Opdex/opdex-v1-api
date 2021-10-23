using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveCirrusTransactionByHashQuery : IRequest<Transaction>
    {
        public RetrieveCirrusTransactionByHashQuery(Sha256 txHash)
        {
            TxHash = txHash;
        }

        public Sha256 TxHash { get; }
    }
}
