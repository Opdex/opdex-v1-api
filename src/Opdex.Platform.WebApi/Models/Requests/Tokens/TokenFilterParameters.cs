using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Tokens
{
    public class TokenFilterParameters : FilterParameters<TokensCursor>
    {
        public TokenFilterParameters()
        {
            Tokens = new List<Address>();
        }

        /// <summary>
        /// The order to sort records by.
        /// </summary>
        public TokenOrderByType OrderBy { get; set; }

        /// <summary>
        /// The type of token to filter for, liquidity pool tokens or not.
        /// </summary>
        public TokenProvisionalFilter Provisional { get; set; }

        /// <summary>
        /// Tokens to filter specifically for.
        /// </summary>
        public IEnumerable<Address> Tokens { get; set; }

        /// <summary>
        /// A generic keyword search against token addresses, names and ticker symbols.
        /// </summary>
        public string Keyword { get; set; }

        /// <inheritdoc />
        protected override TokensCursor InternalBuildCursor()
        {
            if (Cursor is null) return new TokensCursor(Keyword, Tokens, Provisional, OrderBy, Direction, Limit, PagingDirection.Forward, default);
            Cursor.TryBase64Decode(out var decodedCursor);
            TokensCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
