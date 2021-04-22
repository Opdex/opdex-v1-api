using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveCirrusTransactionByHashQuery : IRequest<Transaction>
    {
        public RetrieveCirrusTransactionByHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get; }
    }
}