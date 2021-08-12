using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class TransactionQuoteRequest
    {
        public TransactionQuoteRequest(string sender, string to, string amount, string method, string callback,
                                       IReadOnlyCollection<TransactionQuoteRequestParameter> parameters = null)
        {
            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} must not be null or empty.");
            }

            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to), $"{nameof(to)} must not be null or empty.");
            }

            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException($"{nameof(amount)} must be a valid decimal number", nameof(amount));
            }

            if (!method.HasValue())
            {
                throw new ArgumentNullException(nameof(method), $"{nameof(method)} must not be null or empty.");
            }

            if (!callback.HasValue())
            {
                throw new ArgumentNullException(nameof(callback), $"{nameof(callback)} must not be null or empty.");
            }

            Sender = sender;
            To = to;
            Amount = amount;
            Method = method;
            Callback = callback;
            Parameters = parameters ?? new List<TransactionQuoteRequestParameter>();
        }

        public string Sender { get; }
        public string To { get; }
        public string Amount { get; }
        public string Method { get; }
        public IReadOnlyCollection<TransactionQuoteRequestParameter> Parameters { get; }
        public string[] SerializedParameters => Parameters.Select(p => p.Serialized).ToArray();
        public string Callback { get; }
    }
}
