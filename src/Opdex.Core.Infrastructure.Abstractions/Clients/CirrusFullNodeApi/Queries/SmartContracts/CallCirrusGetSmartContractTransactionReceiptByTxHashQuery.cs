using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetSmartContractTransactionReceiptByTxHashQuery : IRequest<TransactionReceiptDto>
    {
        public CallCirrusGetSmartContractTransactionReceiptByTxHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get;}
    }
}