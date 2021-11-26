using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances
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
