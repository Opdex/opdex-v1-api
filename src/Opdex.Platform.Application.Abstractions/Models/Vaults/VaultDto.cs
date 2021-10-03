using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults
{
    public class VaultDto
    {
        public Address Address { get; set; }
        public Address PendingOwner { get; set; }
        public Address Owner { get; set; }
        public ulong Genesis { get; set; }
        public FixedDecimal TokensLocked { get; set; }
        public FixedDecimal TokensUnassigned { get; set; }
        public Address LockedToken { get; set; }
    }
}
