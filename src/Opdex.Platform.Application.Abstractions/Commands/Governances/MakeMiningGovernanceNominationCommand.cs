using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Governances
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
