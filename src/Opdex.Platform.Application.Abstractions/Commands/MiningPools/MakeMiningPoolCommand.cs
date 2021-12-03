using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningPools;

/// <summary>
/// Create a make mining pool command to persist a mining pool domain model. Include refresh parameters to refresh the
/// included properties prior to persistence.
/// </summary>
public class MakeMiningPoolCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to create a make mining pool command.
    /// </summary>
    /// <param name="miningPool">The mining pool domain model to update and/or persist.</param>
    /// <param name="blockHeight">The block height used to refresh select properties when applicable based on associated refresh parameters.</param>
    /// <param name="refreshRewardPerBlock">Flag to refresh the reward per block value from contract state, default is false.</param>
    /// <param name="refreshRewardPerLpt">Flag to refresh the reward per liquidity pool token value from contract state, default is false.</param>
    /// <param name="refreshMiningPeriodEndBlock">Flag to refresh the mining period end block value from contract state, default is false.</param>
    public MakeMiningPoolCommand(MiningPool miningPool, ulong blockHeight, bool refreshRewardPerBlock = false,
                                 bool refreshRewardPerLpt = false, bool refreshMiningPeriodEndBlock = false)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        MiningPool = miningPool ?? throw new ArgumentNullException(nameof(miningPool), "Mining pool must be provided.");
        BlockHeight = blockHeight;
        RefreshRewardPerBlock = refreshRewardPerBlock;
        RefreshRewardPerLpt = refreshRewardPerLpt;
        RefreshMiningPeriodEndBlock = refreshMiningPeriodEndBlock;
    }

    public MiningPool MiningPool { get; }
    public ulong BlockHeight { get; }
    public bool RefreshRewardPerBlock { get; }
    public bool RefreshRewardPerLpt { get; }
    public bool RefreshMiningPeriodEndBlock { get; }
    public bool Refresh => RefreshRewardPerBlock || RefreshRewardPerLpt || RefreshMiningPeriodEndBlock;
}