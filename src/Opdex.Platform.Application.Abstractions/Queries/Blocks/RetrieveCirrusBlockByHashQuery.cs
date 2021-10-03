using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    /// <summary>
    /// Retrieve a block receipt by block hash from Cirrus.
    /// </summary>
    public class RetrieveCirrusBlockByHashQuery : FindQuery<BlockReceipt>
    {
        /// <summary>
        /// Constructor to create the retrieve cirrus block by hash query.
        /// </summary>
        /// <param name="hash">The block hash to look up the block by.</param>
        /// <param name="findOrThrow">Flag determining if no result is found, to throw not found or return null.</param>
        public RetrieveCirrusBlockByHashQuery(string hash, bool findOrThrow = true) : base(findOrThrow)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash), "Block hash must be provided.");
        }

        public string Hash { get; }
    }
}
