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
                throw new ArgumentNullException(nameof(sender));
            }

            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (!method.HasValue())
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (!callback.HasValue())
            {
                throw new ArgumentNullException(nameof(callback));
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
