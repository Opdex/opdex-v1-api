using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetTransactionByHashQuery : IRequest<Transaction>
    {
        public CallCirrusGetTransactionByHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get;}
    }
}