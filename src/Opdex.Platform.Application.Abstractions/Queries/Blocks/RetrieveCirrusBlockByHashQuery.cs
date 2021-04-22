using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
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