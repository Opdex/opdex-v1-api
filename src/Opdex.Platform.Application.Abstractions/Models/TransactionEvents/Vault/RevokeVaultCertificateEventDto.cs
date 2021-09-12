using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class RevokeVaultCertificateEventDto : TransactionEventDto
    {
        public Address Holder { get; set; }
        public FixedDecimal NewAmount { get; set; }
        public FixedDecimal OldAmount { get; set; }
        public ulong VestedBlock { get; set; }
        public override TransactionEventType EventType => TransactionEventType.RevokeVaultCertificateEvent;
    }
}
