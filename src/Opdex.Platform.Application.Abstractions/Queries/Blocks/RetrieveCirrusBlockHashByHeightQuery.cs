using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusBlockHashByHeightQuery : IRequest<string>
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