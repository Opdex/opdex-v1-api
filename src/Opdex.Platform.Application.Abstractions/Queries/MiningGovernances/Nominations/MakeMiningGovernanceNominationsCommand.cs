using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;

public class MakeMiningGovernanceNominationsCommand : IRequest<bool>
{
    public MakeMiningGovernanceNominationsCommand(MiningGovernance miningGovernance, ulong blockHeight)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance), "Mining governance must be provided.");
        BlockHeight = blockHeight;
    }

    public MiningGovernance MiningGovernance { get; }
    public ulong BlockHeight { get; }
}