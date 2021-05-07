using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class TokenDistribution
    {
        public TokenDistribution(long id, long tokenId, long miningGovernanceId, string owner, ulong genesis, ulong periodDuration, int periodIndex)
        {
            Id = id;
            TokenId = tokenId;
            MiningGovernanceId = miningGovernanceId;
            Owner = owner;
            Genesis = genesis;
            PeriodDuration = periodDuration;
            PeriodIndex = periodIndex;
        }
        
        public TokenDistribution(long tokenId, long miningGovernanceId, string owner, ulong genesis, ulong periodDuration, int periodIndex)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }
            
            if (miningGovernanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningGovernanceId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (genesis < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(genesis));
            }
            
            if (periodDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(periodDuration));
            }

            TokenId = tokenId;
            MiningGovernanceId = miningGovernanceId;
            Owner = owner;
            Genesis = genesis;
            PeriodDuration = periodDuration;
            PeriodIndex = periodIndex;
        }

        public long Id { get; }
        public long TokenId { get; }
        public long MiningGovernanceId { get; }
        public string Owner { get; }
        public ulong Genesis { get; }
        public ulong PeriodDuration { get; }
        public int PeriodIndex { get; }
    }
}