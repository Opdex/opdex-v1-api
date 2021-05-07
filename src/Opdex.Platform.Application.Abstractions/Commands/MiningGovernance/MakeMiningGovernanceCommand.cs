using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningGovernance
{
    public class MakeMiningGovernanceCommand : IRequest<long>
    {
        public MakeMiningGovernanceCommand(Domain.MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }
        
        public Domain.MiningGovernance MiningGovernance { get; }
    }
}