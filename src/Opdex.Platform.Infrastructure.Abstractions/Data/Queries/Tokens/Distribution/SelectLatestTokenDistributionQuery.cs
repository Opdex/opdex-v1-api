using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution
{
    public class SelectLatestTokenDistributionQuery : IRequest<TokenDistribution>
    {
    }
}