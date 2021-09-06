using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public SelectVaultCertificatesWithFilterQuery(long vaultId, VaultCertificatesCursor cursor)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public long VaultId { get; }
        public VaultCertificatesCursor Cursor { get; }
    }
}
