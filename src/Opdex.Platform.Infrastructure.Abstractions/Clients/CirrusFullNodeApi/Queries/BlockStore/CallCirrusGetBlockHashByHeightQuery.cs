using System;
using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockHashByHeightQuery : IRequest<string>
    {
        public CallCirrusGetBlockHashByHeightQuery(ulong height)
        {
            if (height < 1)
            {
                throw new ArgumentNullException(nameof(height));
            }

            Height = height;
        }
        
        public ulong Height { get; }
    }
}