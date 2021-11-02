using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries
{
    /// <summary>
    /// Retrieve a STRAX price quote from Coin Market Cap at a given point in time.
    /// </summary>
    public class RetrieveCmcStraxPriceQuery : IRequest<decimal>
    {
        /// <summary>
        /// Constructor to build a retrieve CMC STRAX price query.
        /// </summary>
        /// <param name="blockTime">The block time to get STRAX USD price at.</param>
        public RetrieveCmcStraxPriceQuery(DateTime blockTime)
        {
            BlockTime = !blockTime.Equals(default)
                ? blockTime
                : throw new ArgumentOutOfRangeException(nameof(blockTime), "Block time must be set.");
        }

        public DateTime BlockTime { get; }
    }
}
