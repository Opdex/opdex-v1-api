using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances
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
