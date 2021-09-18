using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations
{
    public class RetrieveActiveGovernanceNominationsByGovernanceIdQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        public RetrieveActiveGovernanceNominationsByGovernanceIdQuery(long governanceId)
        {
            if (governanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(governanceId), "Governance Id must be greater than 0.");
            }

            GovernanceId = governanceId;
        }

        public long GovernanceId { get; }
    }
}
