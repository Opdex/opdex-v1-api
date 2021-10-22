using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetBlockHashByHeightQuery : IRequest<Sha256>
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
