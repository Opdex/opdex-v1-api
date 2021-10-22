using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetTransactionByHashQuery : IRequest<Transaction>
    {
        public CallCirrusGetTransactionByHashQuery(Sha256 txHash)
        {
            TxHash = txHash;
        }

        public Sha256 TxHash { get; }
    }
}
