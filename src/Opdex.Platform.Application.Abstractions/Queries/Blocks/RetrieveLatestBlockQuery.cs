using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveLatestBlockQuery : FindQuery<Block>
    {
        public RetrieveLatestBlockQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}
