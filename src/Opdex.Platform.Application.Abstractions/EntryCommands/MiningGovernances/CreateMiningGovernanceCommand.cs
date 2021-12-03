using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;

/// <summary>
/// Create a new mining governance record when one does not already exist.
/// </summary>
public class CreateMiningGovernanceCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to create a mining governance command.
    /// </summary>
    /// <param name="miningGovernance">The mining governance contract address.</param>
    /// <param name="stakingTokenId">The staking or mined token in the mining governance.</param>
    /// <param name="blockHeight">The block height the mining governance was created at.</param>
    public CreateMiningGovernanceCommand(Address miningGovernance, ulong stakingTokenId, ulong blockHeight)
    {
        if (miningGovernance == Address.Empty)
        {
            throw new ArgumentNullException(nameof(miningGovernance), "Mining governance address must be provided.");
        }

        if (stakingTokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stakingTokenId), "Staking token id must be greater than zero.");
        }

        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        MiningGovernance = miningGovernance;
        BlockHeight = blockHeight;
        StakingTokenId = stakingTokenId;
    }

    public Address MiningGovernance { get; }
    public ulong StakingTokenId { get; }
    public ulong BlockHeight { get; }
}