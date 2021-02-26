using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.Transaction;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries
{
    public class SelectTransactionByHashQuery : IRequest<Transaction>
    {
        public SelectTransactionByHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }

        public string TxHash { get; }
    }
}