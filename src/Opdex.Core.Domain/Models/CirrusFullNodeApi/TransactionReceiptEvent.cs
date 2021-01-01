using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.CirrusFullNodeApi
{
    public class TransactionReceiptEvent : ITransactionReceiptEvent
    {
        public TransactionReceiptEvent(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
        }

        public string Address { get; }
    }
}