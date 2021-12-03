using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;

public class PersistMiningGovernanceCommand : IRequest<ulong>
{
    public PersistMiningGovernanceCommand(MiningGovernance miningGovernance)
    {
        MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
    }

    public MiningGovernance MiningGovernance { get; }
}