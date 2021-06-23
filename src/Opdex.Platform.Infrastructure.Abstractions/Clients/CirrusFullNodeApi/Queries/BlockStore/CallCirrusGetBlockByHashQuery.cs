using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockByHashQuery : IRequest<BlockReceipt>
    {
        public CallCirrusGetBlockByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
        }

        public string Hash { get;}
    }
}