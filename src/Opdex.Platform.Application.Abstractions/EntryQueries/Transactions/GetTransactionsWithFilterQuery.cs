using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsWithFilterQuery : EntryFilterQuery<TransactionsDto>
    {
        private const uint MaxLimit = 100;

        public GetTransactionsWithFilterQuery(string wallet, IEnumerable<TransactionEventType> eventTypes, IEnumerable<string> contracts,
                                              SortDirectionType direction, uint limit, string next, string previous)
            : base(direction, limit, MaxLimit, next, previous)
        {
            var walletRequest = IsNewRequest ? wallet : CursorProperties.TryGetCursorProperty<string>(nameof(wallet));
            var eventTypesRequest = IsNewRequest ? eventTypes : CursorProperties.TryGetCursorProperties<TransactionEventType>(nameof(eventTypes));
            var contractsRequest = IsNewRequest ? contracts : CursorProperties.TryGetCursorProperties<string>(nameof(contracts));

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
            EventTypes = eventTypesRequest;
            Contracts = contractsRequest;
        }

        public string Wallet { get; }
        public IEnumerable<TransactionEventType> EventTypes { get; }
        public IEnumerable<string> Contracts { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
