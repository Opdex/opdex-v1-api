using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets
{
    public class StakingPositionFilterParameters : FilterParameters<StakingPositionsCursor>
    {
        public StakingPositionFilterParameters()
        {
            LiquidityPools = new List<Address>();
        }

        /// <summary>
        /// The specific liquidity pools to include.
        /// </summary>
        public IEnumerable<Address> LiquidityPools { get; set; }

        /// <summary>
        /// Only includes 0 amounts if true. Default false.
        /// </summary>
        public bool IncludeZeroAmounts { get; set; }

        /// <inheritdoc />
        protected override StakingPositionsCursor InternalBuildCursor()
        {
            if (Cursor is null) return new StakingPositionsCursor(LiquidityPools, IncludeZeroAmounts, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            StakingPositionsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
