using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    /// <summary>
    /// Retrieves a mining pool from its id
    /// </summary>
    public class RetrieveMiningPoolByIdQuery : FindQuery<MiningPool>
    {
        public RetrieveMiningPoolByIdQuery(long miningPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            MiningPoolId = miningPoolId;
        }

        public long MiningPoolId { get; }
    }
}
