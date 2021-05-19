using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveBlockByHeightQuery: FindQuery<Block>
    {
        public RetrieveBlockByHeightQuery(ulong height, bool findOrThrow = true) : base(findOrThrow)
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