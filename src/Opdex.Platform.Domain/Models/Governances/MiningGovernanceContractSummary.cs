using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceContractSummary
    {
        public MiningGovernanceContractSummary(string address, ulong nominationPeriodEnd, uint miningPoolsFunded, string miningPoolReward, ulong miningDuration)
        {
            if (!address.HasValue())
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
        }

        public string Address { get; }
        public ulong NominationPeriodEnd { get; }
        public uint MiningPoolsFunded { get; }
        public string MiningPoolReward { get; }
        public ulong MiningDuration { get; }
    }
}
