using NJsonSchema.Annotations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Transactions
{
    public sealed class TransactionFilterParameters : FilterParameters<TransactionsCursor>
    {
        public TransactionFilterParameters()
        {
            Contracts = new List<Address>();
            EventTypes = new List<TransactionEventType>();
        }

        /// <summary>
        /// Optionally filter transactions by wallet address.
        /// </summary>
        public Address Wallet { get; set; }

        /// <summary>
        /// Optional list of smart contract addresses to filter transactions by.
        /// </summary>
        [NotNull]
        public IEnumerable<Address> Contracts { get; set; }

        /// <summary>
        /// Filter transactions based on event types included.
        /// </summary>
        [NotNull]
        public IEnumerable<TransactionEventType> EventTypes { get; set; }

        protected override TransactionsCursor InternalBuildCursor()
        {
            if (Cursor is null) return new TransactionsCursor(Wallet, EventTypes, Contracts, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            TransactionsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
