using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;

public class TransferEventDto : TransactionEventDto
{
    public Address From { get; set; }
    public Address To { get; set; }
    public FixedDecimal Amount { get; set; }
    public override TransactionEventType EventType => TransactionEventType.TransferEvent;
}