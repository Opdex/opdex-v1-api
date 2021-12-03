using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools;

/// <summary>
/// Retrieves a mining pool from its id
/// </summary>
public class RetrieveMiningPoolByIdQuery : FindQuery<MiningPool>
{
    public RetrieveMiningPoolByIdQuery(ulong miningPoolId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (miningPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
        }

        MiningPoolId = miningPoolId;
    }

    public ulong MiningPoolId { get; }
}