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
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Wallet { get; set; }

        /// <summary>
        /// Optional list of smart contract addresses to filter transactions by.
        /// </summary>
        /// <example>[ "tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i" ]</example>
        [NotNull]
        public IEnumerable<Address> Contracts { get; set; }

        /// <summary>
        /// Filter transactions based on event types included.
        /// </summary>
        /// <example>[ "CreateVaultCertificateEvent", "RevokeVaultCertificateEvent", "RedeemVaultCertificateEvent" ]</example>
        [NotNull]
        public IEnumerable<TransactionEventType> EventTypes { get; set; }

        protected override TransactionsCursor InternalBuildCursor()
        {
            if (EncodedCursor is null) return new TransactionsCursor(Wallet, EventTypes, Contracts, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
            TransactionsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
