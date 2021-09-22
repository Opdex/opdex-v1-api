using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.MiningPools
{
    public class MakeMiningPoolCommand : IRequest<long>
    {
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
}
