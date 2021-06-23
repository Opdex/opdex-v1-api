using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusBlockByHashQuery : IRequest<BlockReceipt>
    {
        public RetrieveCirrusBlockByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
        }

        public string Hash { get; }
    }
}