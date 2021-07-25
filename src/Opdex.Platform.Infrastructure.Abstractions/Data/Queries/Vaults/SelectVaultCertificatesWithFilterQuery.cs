using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public SelectVaultCertificatesWithFilterQuery(long vaultId, string holder, SortDirectionType direction, uint limit, long next, long previous)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
            Holder = holder;
            Direction = direction.IsValid() ? direction : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid sort direction.");
            Limit = limit > 0 ? limit : throw new ArgumentOutOfRangeException(nameof(direction), "Limit cannot be zero.");
            Next = next >= 0 ? next : throw new ArgumentOutOfRangeException(nameof(next), "Next cursor cannot be less than zero.");
            Previous = previous >= 0 ? previous : throw new ArgumentOutOfRangeException(nameof(next), "Previous cursor cannot be less than zero.");
        }

        public long VaultId { get; }
        public string Holder { get; }
        public SortDirectionType Direction { get; }
        public uint Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
