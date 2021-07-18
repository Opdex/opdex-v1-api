using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsWithFilterQuery : EntryFilterQuery<TransactionsDto>
    {
        private const uint MaxLimit = 100;

        public GetTransactionsWithFilterQuery(string wallet, IEnumerable<TransactionEventType> eventTypes, IEnumerable<string> contracts,
                                              string direction, uint limit, string next, string previous)
            : base(direction, limit, MaxLimit, next, previous)
        {
            var walletRequest = IsNewQuery ? wallet : TryGetCursorDictionarySingle(nameof(wallet));
            var eventTypesRequest = IsNewQuery ? eventTypes : TryGetCursorDictionaryList<TransactionEventType>(nameof(eventTypes));
            var contractsRequest = IsNewQuery ? contracts : TryGetCursorDictionaryList<string>(nameof(contracts));

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

            PreviousParsed = previousParsed;
            NextParsed = nextParsed;
            Wallet = walletRequest;
            EventTypes = eventTypesRequest;
            Contracts = contractsRequest;
        }

        public string Wallet { get; }
        public IEnumerable<TransactionEventType> EventTypes { get; }
        public IEnumerable<string> Contracts { get; }
        public long NextParsed { get; }
        public long PreviousParsed { get; }
    }
}
