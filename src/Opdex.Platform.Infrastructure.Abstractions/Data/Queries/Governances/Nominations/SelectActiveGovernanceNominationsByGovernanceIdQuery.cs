using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances.Nominations
{
    public class SelectActiveGovernanceNominationsByGovernanceIdQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        public SelectActiveGovernanceNominationsByGovernanceIdQuery(ulong governanceId)
        {
            if (governanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(governanceId), "Governance Id must be greater than 0.");
            }

            GovernanceId = governanceId;
        }

        public ulong GovernanceId { get; }
    }
}
