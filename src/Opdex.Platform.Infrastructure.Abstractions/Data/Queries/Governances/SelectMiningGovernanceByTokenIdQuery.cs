using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectMiningGovernanceByTokenIdQuery : FindQuery<MiningGovernance>
    {
        public SelectMiningGovernanceByTokenIdQuery(long tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0");
            }

            TokenId = tokenId;
        }

        public long TokenId { get; }
    }
}
