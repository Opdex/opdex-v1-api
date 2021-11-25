using Opdex.Platform.WebApi.Models.Responses.OHLC;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    /// <summary>
    /// Token snapshot details.
    /// </summary>
    public class TokenSnapshotResponseModel
    {
        /// <summary>
        /// The OHLC (open, high, low, close) price of the token's snapshot timeframe and price.
        /// </summary>
        [NotNull]
        public OhlcDecimalResponseModel Price { get; set; }

        /// <summary>
        /// The start time for the snapshot.
        /// </summary>
        /// <example>2022-01-01T00:00:00Z</example>
        [NotNull]
        public DateTime Timestamp { get; set; }
    }
}
