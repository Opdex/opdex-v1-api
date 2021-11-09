using NJsonSchema.Annotations;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolSnapshotResponseModel : LiquidityPoolSummaryResponseModel
    {
        /// <summary>
        /// The start time for the snapshot.
        /// </summary>
        [NotNull]
        public DateTime Timestamp { get; set; }
    }
}
