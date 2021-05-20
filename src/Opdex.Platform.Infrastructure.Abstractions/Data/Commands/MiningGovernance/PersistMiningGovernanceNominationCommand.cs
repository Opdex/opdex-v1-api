using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance
{
    public class PersistMiningGovernanceNominationCommand : IRequest<long>
    {
        public PersistMiningGovernanceNominationCommand(MiningGovernanceNomination nomination)
        {
            Nomination = nomination ?? throw new ArgumentNullException(nameof(nomination));
        }
        
        public MiningGovernanceNomination Nomination { get; }
    }
}