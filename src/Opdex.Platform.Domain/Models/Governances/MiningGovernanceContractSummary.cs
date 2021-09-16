using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceContractSummary
    {
        public MiningGovernanceContractSummary(Address address, ulong nominationPeriodEnd, uint miningPoolsFunded,
                                               UInt256 miningPoolReward, ulong miningDuration, Address minedToken)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must not be null or empty.");
            }

            if (miningDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningDuration), $"{nameof(miningDuration)} must not be greater than 0.");
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
