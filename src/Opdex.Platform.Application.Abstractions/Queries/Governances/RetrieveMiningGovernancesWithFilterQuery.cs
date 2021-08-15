using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernancesWithFilterQuery : IRequest<IEnumerable<MiningGovernance>>
    {
        public RetrieveMiningGovernancesWithFilterQuery(MiningGovernancesCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public MiningGovernancesCursor Cursor { get; }
    }
}
