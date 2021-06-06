using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class MiningPoolSmartContractSummary
    {
        public MiningPoolSmartContractSummary(string address, string miningTokenAddress, string rewardRate, ulong miningPeriodEnd)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (!miningTokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(miningTokenAddress), "Mining token address must be set.");
            }

            if (!rewardRate.IsNumeric())
            {
                throw new ArgumentNullException(nameof(rewardRate), "Reward rate must only contain numeric digits.");
            }

            Address = address;
            MiningTokenAddress = miningTokenAddress;
            RewardRate = rewardRate;
            MiningPeriodEnd = miningPeriodEnd;
        }

        public string Address { get; }
        public string MiningTokenAddress { get; }
        public string RewardRate { get; }
        public ulong MiningPeriodEnd { get; }
    }
}