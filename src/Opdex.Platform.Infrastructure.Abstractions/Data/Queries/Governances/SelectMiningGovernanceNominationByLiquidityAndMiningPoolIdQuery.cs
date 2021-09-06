using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery : FindQuery<MiningGovernanceNomination>
    {
        public SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(long governanceId, long liquidityPoolId, long miningPoolId, bool findOrThrow = true)
            : base(findOrThrow)
        {
            if (governanceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(governanceId), "Governance Id must be greater than 0");
            }

            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity Pool Id must be greater than 0");
            }

            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining Pool Id must be greater than 0");
            }

            GovernanceId = governanceId;
            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
        }

        public long GovernanceId { get; }
        public long LiquidityPoolId { get; }
        public long MiningPoolId { get; }
    }
}
