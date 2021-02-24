using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries
{
    public class SelectTransactionByHashQuery : IRequest<TransactionReceipt>
    {
        public SelectTransactionByHashQuery(string txHash)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
        }

        public string TxHash { get; }
    }
}