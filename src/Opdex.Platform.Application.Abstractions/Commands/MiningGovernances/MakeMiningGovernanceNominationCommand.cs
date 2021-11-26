using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningGovernances
{
    public class MakeMiningGovernanceNominationCommand : IRequest<ulong>
    {
        public MakeMiningGovernanceNominationCommand(MiningGovernanceNomination nomination)
        {
            Nomination = nomination ?? throw new ArgumentNullException(nameof(nomination), "Nomination must be provided.");
        }

        public MiningGovernanceNomination Nomination { get; }
    }
}
