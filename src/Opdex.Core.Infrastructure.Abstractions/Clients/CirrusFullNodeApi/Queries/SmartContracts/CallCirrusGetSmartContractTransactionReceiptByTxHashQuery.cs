using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetSmartContractTransactionReceiptByTxHashQuery : IRequest<TransactionReceipt>
    {
        public CallCirrusGetSmartContractTransactionReceiptByTxHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get;}
    }
}