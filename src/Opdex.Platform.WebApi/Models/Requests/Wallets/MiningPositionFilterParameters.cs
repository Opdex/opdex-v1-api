using NJsonSchema.Annotations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets
{
    public class MiningPositionFilterParameters : FilterParameters<MiningPositionsCursor>
    {
        public MiningPositionFilterParameters()
        {
            // https://github.com/dotnet/aspnetcore/issues/37630 will not be bound if assigned as Enumerable.Empty<T>()
            MiningPools = new List<Address>();
            LiquidityPools = new List<Address>();
        }

        /// <summary>
        /// The specific mining pools to include.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> MiningPools { get; set; }

        /// <summary>
        /// The specific liquidity pools to include.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> LiquidityPools { get; set; }

        /// <summary>
        /// Includes zero amounts if true, otherwise filters out zero amounts if false. Default false.
        /// </summary>
        public bool IncludeZeroAmounts { get; set; }

        /// <inheritdoc />
        protected override MiningPositionsCursor InternalBuildCursor()
        {
            if (Cursor is null) return new MiningPositionsCursor(LiquidityPools, MiningPools, IncludeZeroAmounts, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            MiningPositionsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
