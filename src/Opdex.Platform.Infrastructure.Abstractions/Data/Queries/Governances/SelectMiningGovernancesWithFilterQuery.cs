using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectMiningGovernancesWithFilterQuery : IRequest<IEnumerable<MiningGovernance>>
    {
        public SelectMiningGovernancesWithFilterQuery(MiningGovernancesCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public MiningGovernancesCursor Cursor { get; }
    }
}
