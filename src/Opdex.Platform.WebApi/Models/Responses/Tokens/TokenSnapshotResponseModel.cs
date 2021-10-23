using NJsonSchema.Annotations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotResponseModel
    {
        /// <summary>
        /// The OHLC (open, high, low, close) price of the token's snapshot timeframe and price.
        /// </summary>
        [NotNull]
        public OhlcDecimalResponseModel Price { get; set; }

        /// <summary>
        /// The type of snapshot.
        /// </summary>
        [NotNull]
        public SnapshotType SnapshotType { get; set; }

        /// <summary>
        /// The start date of the snapshot's time period.
        /// </summary>
        [NotNull]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the snapshot's time period.
        /// </summary>
        [NotNull]
        public DateTime EndDate { get; set; }
    }
}
