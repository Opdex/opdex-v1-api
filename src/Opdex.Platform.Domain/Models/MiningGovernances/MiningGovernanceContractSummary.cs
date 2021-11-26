using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.MiningGovernances
{
    public class MiningGovernanceContractSummary
    {
        public MiningGovernanceContractSummary(ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
        public Address? MinedToken { get; private set; }
        public ulong? NominationPeriodEnd { get; private set; }
        public uint? MiningPoolsFunded { get; private set; }
        public UInt256? MiningPoolReward { get; private set; }
        public ulong? MiningDuration { get; private set; }

        public void SetMinedToken(SmartContractMethodParameter value)
        {
            var token = value.Parse<Address>();

            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Mined token address must be provided.");
            }

            MinedToken = token;
        }

        public void SetNominationPeriodEnd(SmartContractMethodParameter value)
        {
            // Zero is valid, nothing to check
            NominationPeriodEnd = value.Parse<ulong>();
        }

        public void SetMiningPoolsFunded(SmartContractMethodParameter value)
        {
            // Zero is valid, nothing to check
            MiningPoolsFunded = value.Parse<uint>();
        }

        public void SetMiningPoolReward(SmartContractMethodParameter value)
        {
            // Zero is valid, nothing to check
            MiningPoolReward = value.Parse<UInt256>();
        }

        public void SetMiningDuration(SmartContractMethodParameter value)
        {
            var miningDuration = value.Parse<ulong>();

            if (miningDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningDuration), "Mining duration must be greater than zero.");
            }

            MiningDuration = miningDuration;
        }
    }
}
