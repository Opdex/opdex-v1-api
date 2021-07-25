using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public RetrieveVaultCertificatesWithFilterQuery(long vaultId, VaultCertificatesCursor cursor)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public long VaultId { get; }
        public VaultCertificatesCursor Cursor { get; }
    }
}
