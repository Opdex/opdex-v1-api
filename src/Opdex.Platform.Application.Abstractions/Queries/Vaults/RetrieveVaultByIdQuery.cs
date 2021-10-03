using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultByIdQuery : FindQuery<Vault>
    {
        public RetrieveVaultByIdQuery(ulong vaultId, bool findOrThrow = true) : base(findOrThrow)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault Id must be greater than zero.");
        }

        public ulong VaultId { get; }
    }
}
