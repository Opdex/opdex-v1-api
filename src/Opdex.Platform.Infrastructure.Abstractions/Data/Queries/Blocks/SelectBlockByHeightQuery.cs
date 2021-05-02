using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks
{
    public class SelectBlockByHeightQuery : IRequest<Block>
    {
        public SelectBlockByHeightQuery(ulong height)
        {
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Height = height;
        }
        
        public ulong Height { get; }
    }
}