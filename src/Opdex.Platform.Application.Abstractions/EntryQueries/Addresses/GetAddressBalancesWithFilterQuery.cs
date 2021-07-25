using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressBalancesWithFilterQuery : EntryFilterQuery<AddressBalancesDto>
    {
        private const uint MaxLimit = 100;

        public GetAddressBalancesWithFilterQuery(string wallet, IEnumerable<string> tokens, bool includeLpTokens, bool includeZeroBalances,
                                                 SortDirectionType direction, uint limit, string next, string previous)
            : base(direction, limit, MaxLimit, next, previous)
        {
            var walletRequest = IsNewRequest ? wallet : CursorProperties.TryGetCursorProperty<string>(nameof(wallet));
            var tokensRequest = IsNewRequest ? tokens : CursorProperties.TryGetCursorProperties<string>(nameof(tokens));
            var includeLpTokensRequest = IsNewRequest ? includeLpTokens : CursorProperties.TryGetCursorProperty<bool>(nameof(includeLpTokens));
            var includeZeroBalancesRequest = IsNewRequest ? includeZeroBalances : CursorProperties.TryGetCursorProperty<bool>(nameof(includeZeroBalances));

            // Decode the Previous cursor if it's provided and validate the value
            var parsedPrevious = long.TryParse(PreviousDecoded, out var previousParsed);
            if (PreviousDecoded.HasValue() && !parsedPrevious)
            {
                throw new ArgumentOutOfRangeException(nameof(previous), "Invalid previous cursor value.");
            }

            // Decode the Next cursor if it's provided and validate the value
            var parsedNext = long.TryParse(NextDecoded, out var nextParsed);
            if (NextDecoded.HasValue() && !parsedNext)
            {
                throw new ArgumentOutOfRangeException(nameof(next), "Invalid next cursor value.");
            }

            Previous = previousParsed;
            Next = nextParsed;
            Wallet = walletRequest;
            Tokens = tokensRequest;
            IncludeLpTokens = includeLpTokensRequest;
            IncludeZeroBalances = includeZeroBalancesRequest;
        }

        public string Wallet { get; }
        public IEnumerable<string> Tokens { get; }
        public bool IncludeLpTokens { get; }
        public bool IncludeZeroBalances { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
