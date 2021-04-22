using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockByHashQuery : IRequest<BlockReceiptDto>
    {
        public CallCirrusGetBlockByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
        }
        
        public string Hash { get;}
    }
}