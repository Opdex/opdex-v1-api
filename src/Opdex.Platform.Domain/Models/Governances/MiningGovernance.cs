using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernance : BlockAudit
    {
        public MiningGovernance(long id, string address, long tokenId, ulong nominationPeriodEnd, ulong miningDuration, uint miningPoolsFunded,
            string miningPoolReward, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningDuration = miningDuration;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }

        public MiningGovernance(string address, long tokenId, ulong nominationPeriodEnd, ulong miningDuration, uint miningPoolsFunded,
            string miningPoolReward, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
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
            NominationPeriodEnd = nominationPeriodEnd;
            MiningDuration = miningDuration;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }

        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public ulong NominationPeriodEnd { get; private set; }
        public ulong MiningDuration { get; private set; }
        public uint MiningPoolsFunded { get; private set; }
        public string MiningPoolReward { get; private set; }

        public void Update(MiningGovernanceContractSummary summary, ulong block)
        {
            NominationPeriodEnd = summary.NominationPeriodEnd;
            MiningPoolsFunded = summary.MiningPoolsFunded;
            MiningPoolReward = summary.MiningPoolReward;
            MiningDuration = summary.MiningDuration;

            SetModifiedBlock(block);
        }
    }
}
