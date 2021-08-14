using NJsonSchema.Annotations;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults
{
    public class VaultResponseModel
    {
        [NotNull]
        public string Address { get; set; }

        [NotNull]
        public string Owner { get; set; }

        [NotNull]
        public ulong Genesis { get; set; }

        [NotNull]
        public string TokensLocked { get; set; }

        [NotNull]
        public string TokensUnassigned { get; set; }

        [NotNull]
        public string LockedToken { get; set; }
    }
}
