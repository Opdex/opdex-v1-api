using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsWithFilterQuery : EntryFilterQuery<TransactionsDto>
    {
        private const uint MaxLimit = 100;

        public GetTransactionsWithFilterQuery(string wallet, IEnumerable<uint> includeEvents, IEnumerable<uint> excludeEvents,
                                              IEnumerable<string> contracts, string direction, uint limit, string next, string previous)
            : base(direction, limit, MaxLimit, next, previous)
        {
            var walletRequest = IsNewRequest ? wallet : TryGetCursorDictionarySingle(nameof(wallet));
            var includeEventsRequest = IsNewRequest ? includeEvents : TryGetCursorDictionaryList<uint>(nameof(includeEvents));
            var excludeEventsRequest = IsNewRequest ? excludeEvents : TryGetCursorDictionaryList<uint>(nameof(excludeEvents));
            var contractsRequest = IsNewRequest ? contracts : TryGetCursorDictionaryList<string>(nameof(contracts));

            // Todo: Switch events to Enum with validation if they're provided

            // Decode the Next and Previous strings to make sure they're expected value types
            var parsedPrevious = long.TryParse(PreviousDecoded, out var previousParsed);
            if (PreviousDecoded.HasValue() && !parsedPrevious)
            {
                throw new ArgumentOutOfRangeException(nameof(previous), "Invalid previous cursor value.");
            }

            var parsedNext = long.TryParse(NextDecoded, out var nextParsed);
            if (NextDecoded.HasValue() && !parsedNext)
            {
                throw new ArgumentOutOfRangeException(nameof(next), "Invalid next cursor value.");
            }

            PreviousParsed = previousParsed;
            NextParsed = nextParsed;
            Wallet = walletRequest;
            IncludeEvents = includeEventsRequest;
            ExcludeEvents = excludeEventsRequest;
            Contracts = contractsRequest;
        }

        public string Wallet { get; }
        public IEnumerable<uint> IncludeEvents { get; }
        public IEnumerable<uint> ExcludeEvents { get; }
        public IEnumerable<string> Contracts { get; }
        public long NextParsed { get; }
        public long PreviousParsed { get; }
    }
}
