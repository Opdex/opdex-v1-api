using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances
{
    public class PersistMiningGovernanceCommand : IRequest<long>
    {
        public PersistMiningGovernanceCommand(MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }

        public MiningGovernance MiningGovernance { get; }
    }
}
