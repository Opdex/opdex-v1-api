using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution
{
    public class SelectLatestTokenDistributionQuery : FindQuery<TokenDistribution>
    {
        public SelectLatestTokenDistributionQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}