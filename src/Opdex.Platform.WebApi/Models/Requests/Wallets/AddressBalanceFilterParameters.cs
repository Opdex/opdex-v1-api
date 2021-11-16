using NJsonSchema.Annotations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets
{
    public class AddressBalanceFilterParameters : FilterParameters<AddressBalancesCursor>
    {
        public AddressBalanceFilterParameters()
        {
            Tokens = new List<Address>();
            TokenType = TokenProvisionalFilter.All;
        }

        /// <summary>
        /// Specific tokens to lookup.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> Tokens { get; set; }

        /// <summary>
        /// The type of token to filter by, either provisional or non-provisional.
        /// </summary>
        public TokenProvisionalFilter TokenType { get; set; }

        /// <summary>
        /// Includes zero balances if true, otherwise filters out zero balances if false. Default false.
        /// </summary>
        public bool IncludeZeroBalances { get; set; }

        /// <inheritdoc />
        protected override AddressBalancesCursor InternalBuildCursor()
        {
            if (EncodedCursor is null) return new AddressBalancesCursor(Tokens, TokenType, IncludeZeroBalances, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
            AddressBalancesCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
