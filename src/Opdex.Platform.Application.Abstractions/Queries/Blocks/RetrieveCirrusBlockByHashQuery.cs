using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusBlockByHashQuery : FindQuery<BlockReceipt>
    {
        public RetrieveCirrusBlockByHashQuery(string hash, bool findOrThrow = true) : base(findOrThrow)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash), "Block hash must be provided.");
        }

        public string Hash { get; }
    }
}
