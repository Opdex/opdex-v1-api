using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;

public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery : FindQuery<MiningGovernanceNomination>
{
    public SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(ulong miningGovernanceId, ulong liquidityPoolId, ulong miningPoolId, bool findOrThrow = true)
        : base(findOrThrow)
    {
        if (miningGovernanceId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(miningGovernanceId), "Mining governance Id must be greater than 0");
        }

        if (liquidityPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity Pool Id must be greater than 0");
        }

        if (miningPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining Pool Id must be greater than 0");
        }

        MiningGovernanceId = miningGovernanceId;
        LiquidityPoolId = liquidityPoolId;
        MiningPoolId = miningPoolId;
    }

    public ulong MiningGovernanceId { get; }
    public ulong LiquidityPoolId { get; }
    public ulong MiningPoolId { get; }
}