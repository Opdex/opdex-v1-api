using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations
{
    public class RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        public RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(ulong miningGovernanceId)
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
