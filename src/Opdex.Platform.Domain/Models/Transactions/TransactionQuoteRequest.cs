using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class TransactionQuoteRequest
    {
        public TransactionQuoteRequest(Address sender, Address to, FixedDecimal amount, string method, string callback,
                                       IReadOnlyCollection<TransactionQuoteRequestParameter> parameters = null)
        {
            if (sender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} must not be null or empty.");
            }

            if (to == Address.Empty)
            {
                throw new ArgumentNullException(nameof(to), $"{nameof(to)} must not be null or empty.");
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
            Parameters = parameters ?? new List<TransactionQuoteRequestParameter>().AsReadOnly();
        }

        public Address Sender { get; }
        public Address To { get; }
        public FixedDecimal Amount { get; }
        public string Method { get; }
        public IReadOnlyCollection<TransactionQuoteRequestParameter> Parameters { get; }
        public SmartContractMethodParameter[] MethodParameters => Parameters.Select(p => p.Value).ToArray();
        public string Callback { get; }
    }
}
