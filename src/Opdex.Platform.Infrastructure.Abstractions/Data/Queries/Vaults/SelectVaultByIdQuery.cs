using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultByIdQuery : FindQuery<Vault>
    {
        public SelectVaultByIdQuery(long vaultId, bool findOrThrow) : base(findOrThrow)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault Id must be greater than zero.");
        }

        public long VaultId { get; }
    }
}
