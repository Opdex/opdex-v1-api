using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.MiningPools
{
    public class MiningPoolSmartContractSummary
    {
        public MiningPoolSmartContractSummary(Address address, Address miningToken, UInt256 rewardRate, ulong miningPeriodEnd)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (miningToken == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningToken), "Mining token address must be set.");
            }

            Address = address;
            MiningToken = miningToken;
            RewardRate = rewardRate;
            MiningPeriodEnd = miningPeriodEnd;
        }

        public Address Address { get; }
        public Address MiningToken { get; }
        public UInt256 RewardRate { get; }
        public ulong MiningPeriodEnd { get; }
    }
}
