using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningGovernance
{
    public class MakeMiningGovernanceCommand : IRequest<long>
    {
        public MakeMiningGovernanceCommand(Domain.Models.MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }
        
        public Domain.Models.MiningGovernance MiningGovernance { get; }
    }
}