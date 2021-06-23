using System;
using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance
{
    public class PersistMiningGovernanceCommand : IRequest<long>
    {
        public PersistMiningGovernanceCommand(Domain.Models.MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }
        
        public Domain.Models.MiningGovernance MiningGovernance { get; set; }
    }
}