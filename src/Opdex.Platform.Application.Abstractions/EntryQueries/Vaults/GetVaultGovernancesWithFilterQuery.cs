using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;

public class GetVaultGovernancesWithFilterQuery : IRequest<VaultGovernancesDto>
{
    public GetVaultGovernancesWithFilterQuery(VaultGovernancesCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultGovernancesCursor Cursor { get; }
}
