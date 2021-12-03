using Opdex.Platform.Domain.Models.TransactionLogs;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Domain.Models.Transactions;

public class TransactionQuote
{
    public TransactionQuote(object result, string error, uint gasUsed, IReadOnlyCollection<TransactionLog> logs, TransactionQuoteRequest request)
    {
        if (gasUsed == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gasUsed), $"{nameof(gasUsed)} must be greater than 0.");
        }

        Result = result;
        Error = error;
        GasUsed = gasUsed;
        Logs = logs ?? new List<TransactionLog>();
        Request = request ?? throw new ArgumentNullException(nameof(request), $"{nameof(request)} must not be null.");
    }

    public object Result { get; }
    public string Error { get; }
    public uint GasUsed { get; }
    public IReadOnlyCollection<TransactionLog> Logs { get; }
    public TransactionQuoteRequest Request { get; }
}