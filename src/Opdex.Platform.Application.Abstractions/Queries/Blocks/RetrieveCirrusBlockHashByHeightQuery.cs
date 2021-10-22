using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusBlockHashByHeightQuery : IRequest<Sha256>
    {
        public RetrieveCirrusBlockHashByHeightQuery(ulong height)
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
