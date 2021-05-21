using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain
{
    public class MiningGovernance : IRequest<Unit>
    {
        public MiningGovernance(long id, string address, long tokenId, ulong nominationPeriodEnd, int miningPoolsFunded, 
            string miningPoolReward, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public MiningGovernance(string address, long tokenId, ulong nominationPeriodEnd, int miningPoolsFunded, 
            string miningPoolReward, ulong createdBlock, ulong modifiedBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }
            
            if (nominationPeriodEnd < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(nominationPeriodEnd));
            }
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }

            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public ulong NominationPeriodEnd { get; }
        public int MiningPoolsFunded { get; }
        public string MiningPoolReward { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}