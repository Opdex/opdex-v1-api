using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockByHashQuery : FindQuery<BlockReceipt>
    {
        public CallCirrusGetBlockByHashQuery(string hash, bool findOrThrow = true) : base(findOrThrow)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash), "Block has must be provided.");
        }

        public string Hash { get;}
    }
}
