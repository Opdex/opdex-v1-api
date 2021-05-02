using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveBlockByHeightQuery: IRequest<Block>
    {
        public RetrieveBlockByHeightQuery(ulong height)
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