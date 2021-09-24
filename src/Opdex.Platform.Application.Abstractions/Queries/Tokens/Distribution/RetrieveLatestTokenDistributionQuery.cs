using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution
{
    public class RetrieveLatestTokenDistributionQuery : FindQuery<TokenDistribution>
    {
        public RetrieveLatestTokenDistributionQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}
