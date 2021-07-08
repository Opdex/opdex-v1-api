using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances
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
