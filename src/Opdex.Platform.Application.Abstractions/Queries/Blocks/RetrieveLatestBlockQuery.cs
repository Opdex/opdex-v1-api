using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveLatestBlockQuery : FindQuery<BlockDto>
    {
        public RetrieveLatestBlockQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}