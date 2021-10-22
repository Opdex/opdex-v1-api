using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    /// <summary>
    /// Retrieve a block receipt by block hash from Cirrus.
    /// </summary>
    public class RetrieveCirrusBlockReceiptByHashQuery : FindQuery<BlockReceipt>
    {
        /// <summary>
        /// Constructor to create the retrieve cirrus block receipt by hash query.
        /// </summary>
        /// <param name="hash">The block hash to look up the block by.</param>
        /// <param name="findOrThrow">Flag determining if no result is found, to throw not found or return null.</param>
        public RetrieveCirrusBlockReceiptByHashQuery(Sha256 hash, bool findOrThrow = true) : base(findOrThrow)
        {
            Hash = hash;
        }

        public Sha256 Hash { get; }
    }
}
