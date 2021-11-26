using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances
{
    public class SelectMiningGovernanceByTokenIdQuery : FindQuery<MiningGovernance>
    {
        public SelectMiningGovernanceByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0");
            }

            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}
