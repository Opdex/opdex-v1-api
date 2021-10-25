using Opdex.Platform.Common.Enums;
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
            Attributes = new List<TokenAttributeType>();

            // Todo: Should this be the default? Should we make a "Default" enum value?
            // Maybe try and make this nullable again and have (null, tokenId) as a valid cursor and remove "AddedBlock" completely.
            OrderBy = TokenOrderByType.AddedBlock;
        }

        /// <summary>
        /// The order to sort records by.
        /// </summary>
        public TokenOrderByType OrderBy { get; set; }

        /// <summary>
        /// Token attributes to filter for.
        /// </summary>
        public IEnumerable<TokenAttributeType> Attributes { get; set; }

        /// <summary>
        /// The liquidity pools used for mining.
        /// </summary>
        public IEnumerable<Address> Tokens { get; set; }

        /// <summary>
        /// A generic keyword search against token addresses, names and ticker symbols.
        /// </summary>
        public string Keyword { get; set; }

        /// <inheritdoc />
        protected override TokensCursor InternalBuildCursor()
        {
            if (Cursor is null) return new TokensCursor(Keyword, Tokens, Attributes, OrderBy, Direction, Limit, PagingDirection.Forward, default);
            Cursor.TryBase64Decode(out var decodedCursor);
            TokensCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
