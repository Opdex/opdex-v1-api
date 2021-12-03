using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Transactions;

public class TransactionQuoteDto
{
    public object Result { get; set; }
    public string Error { get; set; }
    public uint GasUsed { get; set; }
    public IReadOnlyCollection<TransactionEventDto> Events { get; set; }
    public TransactionQuoteRequestDto Request { get; set; }
}