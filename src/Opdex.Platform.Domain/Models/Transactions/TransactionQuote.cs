using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class TransactionQuote
    {
        public TransactionQuote(object result, string error, uint gasUsed, IReadOnlyCollection<TransactionLog> logs, TransactionQuoteRequest request)
        {
            Result = result;
            Error = error;
            Logs = logs ?? new List<TransactionLog>();
            GasUsed = gasUsed > 0 ? gasUsed : throw new ArgumentOutOfRangeException(nameof(gasUsed));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public object Result { get; }
        public string Error { get; }
        public uint GasUsed { get; }
        public IReadOnlyCollection<TransactionLog> Logs { get; }
        public TransactionQuoteRequest Request { get; }
    }
}
