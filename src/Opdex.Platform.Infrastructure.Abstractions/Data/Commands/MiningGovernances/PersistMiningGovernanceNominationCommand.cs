using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;

public class PersistMiningGovernanceNominationCommand : IRequest<ulong>
{
    public PersistMiningGovernanceNominationCommand(MiningGovernanceNomination nomination)
    {
        Nomination = nomination ?? throw new ArgumentNullException(nameof(nomination));
    }

    public MiningGovernanceNomination Nomination { get; }
}