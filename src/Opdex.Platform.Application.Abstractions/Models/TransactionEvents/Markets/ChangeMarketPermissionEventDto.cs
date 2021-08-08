using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class ChangeMarketPermissionEventDto : TransactionEventDto
    {
        public string Address { get; set; }
        public string Permission { get; set; }
        public bool IsAuthorized { get; set; }
        public override TransactionEventType EventType => TransactionEventType.ChangeMarketPermissionEvent;
    }
}
