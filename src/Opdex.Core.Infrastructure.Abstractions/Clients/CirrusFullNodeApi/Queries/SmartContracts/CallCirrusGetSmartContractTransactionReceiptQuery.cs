using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetSmartContractTransactionReceiptQuery : IRequest<ReceiptDto>
    {
        public CallCirrusGetSmartContractTransactionReceiptQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get;}
    }
}