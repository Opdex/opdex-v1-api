using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class CreateVaultCertificateEventDto : TransactionEventDto
    {
        public Address Holder { get; set; }
        public FixedDecimal Amount { get; set; }
        public ulong VestedBlock { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CreateVaultCertificateEvent;
    }
}
