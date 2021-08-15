using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Governances
{
    /// <summary>
    /// Retrieves a paginated collection of mining governances
    /// </summary>
    public class GetMiningGovernancesWithFilterQuery : IRequest<MiningGovernancesDto>
    {
        public GetMiningGovernancesWithFilterQuery(MiningGovernancesCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public MiningGovernancesCursor Cursor { get; }
    }
}
