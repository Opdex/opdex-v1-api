using NJsonSchema.Annotations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.MiningPools
{
    public class MiningPoolFilterParameters : FilterParameters<MiningPoolsCursor>
    {
        public MiningPoolFilterParameters()
        {
            LiquidityPools = new List<Address>();
        }

        /// <summary>
        /// The liquidity pools used for mining.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> LiquidityPools { get; set; }

        /// <summary>
        /// Mining pool activity status.
        /// </summary>
        public MiningStatusFilter MiningStatus { get; set; }

        /// <inheritdoc />
        protected override MiningPoolsCursor InternalBuildCursor()
        {
            if (Cursor is null) return new MiningPoolsCursor(LiquidityPools, MiningStatus, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            MiningPoolsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
