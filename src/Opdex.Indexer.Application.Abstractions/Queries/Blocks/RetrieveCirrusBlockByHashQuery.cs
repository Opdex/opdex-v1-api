using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusBlockByHashQuery : IRequest<BlockReceiptDto>
    {
        public RetrieveCirrusBlockByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
        }
        
        public string Hash { get; }
    }
}