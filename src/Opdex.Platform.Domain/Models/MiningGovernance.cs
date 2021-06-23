using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Domain.Models
{
    public class MiningGovernance : BlockAudit
    {
        public MiningGovernance(long id, string address, long tokenId, ulong nominationPeriodEnd, uint miningPoolsFunded,
            string miningPoolReward, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }

        public MiningGovernance(string address, long tokenId, ulong nominationPeriodEnd, uint miningPoolsFunded,
            string miningPoolReward, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }

        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public ulong NominationPeriodEnd { get; private set; }
        public uint MiningPoolsFunded { get; private set; }
        public string MiningPoolReward { get; private set; }

        public void Update(MiningGovernanceContractSummary summary, ulong block)
        {
            NominationPeriodEnd = summary.NominationPeriodEnd;
            MiningPoolsFunded = summary.MiningPoolsFunded;
            MiningPoolReward = summary.MiningPoolReward;

            SetModifiedBlock(block);
        }
    }
}