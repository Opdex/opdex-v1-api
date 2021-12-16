using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;

public class RetrieveVaultGovernancesWithFilterQuery : IRequest<IEnumerable<VaultGovernance>>
{
    public RetrieveVaultGovernancesWithFilterQuery(VaultGovernancesCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultGovernancesCursor Cursor { get; }
}
