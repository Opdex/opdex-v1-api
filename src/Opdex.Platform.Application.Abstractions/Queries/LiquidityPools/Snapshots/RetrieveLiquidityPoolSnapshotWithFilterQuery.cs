using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Query to retrieve the liquidity pool snapshot type for the provided DateTime or the most recent snapshot prior.
    /// If no snapshot's are found, returns a newly instantiated <see cref="LiquidityPoolSnapshot"/>.
    /// </summary>
    public class RetrieveLiquidityPoolSnapshotWithFilterQuery : IRequest<LiquidityPoolSnapshot>
    {
        /// <summary>
        /// Query to retrieve the liquidity pool snapshot type for the provided DateTime or the most recent snapshot prior.
        /// If no snapshot's are found, returns a newly instantiated <see cref="LiquidityPoolSnapshot"/>.
        /// </summary>
        /// <param name="liquidityPoolId">The internal Id of the liquidity pool.</param>
        /// <param name="dateTime">The date and time of the requested snapshot to be in between.</param>
        /// <param name="snapshotType">The type of snapshot being requested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for invalid liquidityPoolId or snapshotType.</exception>
        public RetrieveLiquidityPoolSnapshotWithFilterQuery(ulong liquidityPoolId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "liquidityPoolId must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            LiquidityPoolId = liquidityPoolId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public ulong LiquidityPoolId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}
