using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents;

public abstract class OwnershipEventDto : TransactionEventDto
{
    public Address From { get; set; }
    public Address To { get; set; }
}