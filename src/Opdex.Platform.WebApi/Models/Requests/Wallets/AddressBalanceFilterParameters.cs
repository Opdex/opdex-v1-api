using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets
{
    public class AddressBalanceFilterParameters : FilterParameters<AddressBalancesCursor>
    {
        public AddressBalanceFilterParameters()
        {
            Tokens = new List<Address>();
            IncludeLpTokens = true;
        }

        /// <summary>
        /// Specific tokens to lookup.
        /// </summary>
        public IEnumerable<Address> Tokens { get; set; }

        /// <summary>
        /// Includes all tokens if true, otherwise excludes liquidity pool tokens if false. Default true.
        /// </summary>
        public bool IncludeLpTokens { get; set; }

        /// <summary>
        /// Includes zero balances if true, otherwise filters out zero balances if false. Default false.
        /// </summary>
        public bool IncludeZeroBalances { get; set; }

        /// <inheritdoc />
        protected override AddressBalancesCursor InternalBuildCursor()
        {
            if (Cursor is null) return new AddressBalancesCursor(Tokens, IncludeLpTokens, IncludeZeroBalances, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            AddressBalancesCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
