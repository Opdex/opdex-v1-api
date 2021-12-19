using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;

public class GetVaultGovernancesWithFilterQuery : IRequest<VaultGovernancesDto>
{
    public GetVaultGovernancesWithFilterQuery(VaultGovernancesCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultGovernancesCursor Cursor { get; }
}
