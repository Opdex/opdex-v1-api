using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceContractSummary
    {
        public MiningGovernanceContractSummary(Address address, Address minedToken, ulong nominationPeriodEnd, uint miningPoolsFunded,
                                               UInt256 miningPoolReward, ulong miningDuration)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Governance address must be provided.");
            }

            if (minedToken == Address.Empty)
            {
                throw new ArgumentNullException(nameof(minedToken), "Governance mined token address must be provided.");
            }

            if (miningDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningDuration), "Mining duration must be greater than zero.");
            }

            Address = address;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
            MiningDuration = miningDuration;
            MinedToken = minedToken;
        }

        public Address Address { get; }
        public Address MinedToken { get; }
        public ulong NominationPeriodEnd { get; }
        public uint MiningPoolsFunded { get; }
        public UInt256 MiningPoolReward { get; }
        public ulong MiningDuration { get; }
    }
}
