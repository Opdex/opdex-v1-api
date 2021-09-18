using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations
{
    public class MakeGovernanceNominationsCommand : IRequest<bool>
    {
        public MakeGovernanceNominationsCommand(MiningGovernance governance, ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Governance = governance ?? throw new ArgumentNullException(nameof(governance), "Governance must be provided.");
            BlockHeight = blockHeight;
        }

        public MiningGovernance Governance { get; }
        public ulong BlockHeight { get; }
    }
}
