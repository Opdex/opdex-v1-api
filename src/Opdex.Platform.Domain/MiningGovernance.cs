using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain
{
    public class MiningGovernance : IRequest<Unit>
    {
        public MiningGovernance(long id, string address, long tokenId, ulong nominationPeriodEnd, string balance, 
            int miningPoolsFunded, string miningPoolReward)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            Balance = balance;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }
        
        public MiningGovernance(string address, long tokenId, ulong nominationPeriodEnd, string balance, 
            int miningPoolsFunded, string miningPoolReward)
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
            
            if (!balance.IsNumeric())
            {
                throw new ArgumentNullException(nameof(balance));
            }

            Address = address;
            TokenId = tokenId;
            NominationPeriodEnd = nominationPeriodEnd;
            Balance = balance;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public ulong NominationPeriodEnd { get; }
        public string Balance { get; }
        public int MiningPoolsFunded { get; }
        public string MiningPoolReward { get; }
    }
}