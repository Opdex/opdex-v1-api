using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;

public class SelectVaultGovernancesWithFilterQuery : IRequest<IEnumerable<VaultGovernance>>
{
    public SelectVaultGovernancesWithFilterQuery(VaultGovernancesCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultGovernancesCursor Cursor { get; }
}
