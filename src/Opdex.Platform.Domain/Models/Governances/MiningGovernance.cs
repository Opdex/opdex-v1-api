using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernance : BlockAudit
    {
        public MiningGovernance(long id, Address address, long tokenId, ulong nominationPeriodEnd, ulong miningDuration, uint miningPoolsFunded,
                                UInt256 miningPoolReward, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningDuration = miningDuration;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }

        public MiningGovernance(Address address, long tokenId, ulong miningDuration, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must not be null or empty.");
            }

            if (miningDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningDuration), $"{nameof(miningDuration)} must be greater than 0.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            Address = address;
            TokenId = tokenId;
            MiningDuration = miningDuration;
        }

        public long Id { get; }
        public Address Address { get; }
        public long TokenId { get; }
        public ulong NominationPeriodEnd { get; private set; }
        public ulong MiningDuration { get; private set; }
        public uint MiningPoolsFunded { get; private set; }
        public UInt256 MiningPoolReward { get; private set; }

        public void Update(MiningGovernanceContractSummary summary)
        {
            if(summary.NominationPeriodEnd.HasValue) NominationPeriodEnd = summary.NominationPeriodEnd.Value;
            if(summary.MiningPoolsFunded.HasValue) MiningPoolsFunded = summary.MiningPoolsFunded.Value;
            if(summary.MiningPoolReward.HasValue) MiningPoolReward = summary.MiningPoolReward.Value;
            if(summary.MiningDuration.HasValue) MiningDuration = summary.MiningDuration.Value;

            SetModifiedBlock(summary.BlockHeight);
        }
    }
}
