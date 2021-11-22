using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

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
        [Range(1, double.MaxValue)]
        public ulong Genesis { get; set; }

        [NotNull]
        public FixedDecimal TokensLocked { get; set; }

        [NotNull]
        public FixedDecimal TokensUnassigned { get; set; }

        [NotNull]
        public Address LockedToken { get; set; }
    }
}
