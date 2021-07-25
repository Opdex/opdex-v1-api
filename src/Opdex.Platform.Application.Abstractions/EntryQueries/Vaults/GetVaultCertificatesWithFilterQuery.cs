using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults
{
    public class GetVaultCertificatesWithFilterQuery : IRequest<VaultCertificatesDto>
    {
        public GetVaultCertificatesWithFilterQuery(string vault, VaultCertificatesCursor cursor)
        {
            Vault = vault.HasValue() ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public string Vault { get; }
        public VaultCertificatesCursor Cursor { get; }
    }
}
