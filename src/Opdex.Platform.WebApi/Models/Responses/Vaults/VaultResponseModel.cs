using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    public class VaultResponseModel
    {
        [NotNull]
        public Address Address { get; set; }

        public Address PendingOwner { get; set; }

        [NotNull]
        public Address Owner { get; set; }

        [NotNull]
        public ulong Genesis { get; set; }

        [NotNull]
        public FixedDecimal TokensLocked { get; set; }

        [NotNull]
        public FixedDecimal TokensUnassigned { get; set; }

        [NotNull]
        public Address LockedToken { get; set; }
    }
}
