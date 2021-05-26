using System;
using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance
{
    public class PersistMiningGovernanceCommand : IRequest<long>
    {
        public PersistMiningGovernanceCommand(Domain.MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }
        
        public Domain.MiningGovernance MiningGovernance { get; set; }
    }
}