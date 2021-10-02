using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks
{
    /// <summary>
    /// Select a block based on a given datetime returning the most recent block that is less than the provided time.
    /// </summary>
    public class SelectBlockByMedianTimeQuery : FindQuery<Block>
    {
        /// <summary>
        /// Constructor to create a select block by median time query.
        /// </summary>
        /// <param name="medianTime">The median block time.</param>
        /// <param name="findOrThrow">Throws not found exception if true and no record is found, default is true.</param>
        public SelectBlockByMedianTimeQuery(DateTime medianTime, bool findOrThrow = true) : base(findOrThrow)
        {
            if (medianTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(medianTime), "Median time must be valid datetime.");
            }

            MedianTime = medianTime;
        }

        public DateTime MedianTime { get; }
    }
}
