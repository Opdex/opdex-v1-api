using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

public class RetrieveVaultGovernancesWithFilterQuery : IRequest<IEnumerable<VaultGovernance>>
{
    public RetrieveVaultGovernancesWithFilterQuery(VaultGovernancesCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultGovernancesCursor Cursor { get; }
}
