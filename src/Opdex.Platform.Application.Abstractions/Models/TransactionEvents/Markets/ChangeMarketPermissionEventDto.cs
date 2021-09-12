using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class ChangeMarketPermissionEventDto : TransactionEventDto
    {
        public Address Address { get; set; }
        public string Permission { get; set; }
        public bool IsAuthorized { get; set; }
        public override TransactionEventType EventType => TransactionEventType.ChangeMarketPermissionEvent;
    }
}
