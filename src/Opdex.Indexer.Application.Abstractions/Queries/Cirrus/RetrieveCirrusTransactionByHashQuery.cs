using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Indexer.Domain.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus
{
    public class RetrieveCirrusTransactionByHashQuery : IRequest<TransactionReceipt>
    {
        public RetrieveCirrusTransactionByHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }
        
        public string TxHash { get; }
    }
}