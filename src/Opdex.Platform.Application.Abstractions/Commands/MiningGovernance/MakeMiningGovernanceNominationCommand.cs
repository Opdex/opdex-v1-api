using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningGovernance
{
    public class MakeMiningGovernanceNominationCommand : IRequest<long>
    {
        public MakeMiningGovernanceNominationCommand(MiningGovernanceNomination nomination)
        {
            Nomination = nomination ?? throw new ArgumentNullException(nameof(nomination));
        }
        
        public MiningGovernanceNomination Nomination { get; }
    }
}