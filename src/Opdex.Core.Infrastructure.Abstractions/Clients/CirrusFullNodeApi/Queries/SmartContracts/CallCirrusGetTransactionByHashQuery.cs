using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
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