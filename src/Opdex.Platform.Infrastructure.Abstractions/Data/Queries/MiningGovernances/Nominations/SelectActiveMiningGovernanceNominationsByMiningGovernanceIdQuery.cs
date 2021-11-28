using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations
{
    public class SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        public SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(ulong miningGovernanceId)
        {
            if (miningGovernanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningGovernanceId), "Mining governance Id must be greater than 0.");
            }

            MiningGovernanceId = miningGovernanceId;
        }

        public ulong MiningGovernanceId { get; }
    }
}
