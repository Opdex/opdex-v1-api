using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus
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