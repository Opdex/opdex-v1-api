using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks
{
    public class SelectLatestBlockQuery : FindQuery<Block>
    {
        public SelectLatestBlockQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}
