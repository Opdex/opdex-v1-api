using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockStoreBlockDetailsByHashQuery : IRequest<BlockDto>
    {
        public CallCirrusGetBlockStoreBlockDetailsByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
        }
        
        public string Hash { get;}
    }
}