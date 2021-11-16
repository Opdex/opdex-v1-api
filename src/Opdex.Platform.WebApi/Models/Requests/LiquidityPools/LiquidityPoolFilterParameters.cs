using NJsonSchema.Annotations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools
{
    public class LiquidityPoolFilterParameters : FilterParameters<LiquidityPoolsCursor>
    {
        public LiquidityPoolFilterParameters()
        {
            Markets = new List<Address>();
            LiquidityPools = new List<Address>();
            Tokens = new List<Address>();
        }

        /// <summary>
        /// A generic keyword search against liquidity pool addresses and names.
        /// </summary>
        [NotNull]
        public string Keyword { get; set; }

        /// <summary>
        /// Markets to search liquidity pools within.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> Markets { get; set; }

        /// <summary>
        /// Liquidity pools to filter specifically for.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> LiquidityPools { get; set; }

        /// <summary>
        /// Tokens to filter specifically for.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> Tokens { get; set; }

        /// <summary>
        /// Staking status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolStakingStatusFilter StakingFilter { get; set; }

        /// <summary>
        /// Nomination status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolNominationStatusFilter NominationFilter { get; set; }

        /// <summary>
        /// Mining status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolMiningStatusFilter MiningFilter { get; set; }

        /// <summary>
        /// The order to sort records by.
        /// </summary>
        public LiquidityPoolOrderByType OrderBy { get; set; }

        /// <inheritdoc />
        protected override LiquidityPoolsCursor InternalBuildCursor()
        {
            if (EncodedCursor is null) return new LiquidityPoolsCursor(Keyword, Markets, LiquidityPools, Tokens, StakingFilter, NominationFilter, MiningFilter,
                                                                       OrderBy, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
            LiquidityPoolsCursor.TryParse(decodedCursor, out var cursor);

            return cursor;
        }
    }
}
