using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveLatestTokenDistributionQuery : IRequest<TokenDistribution>
    {
    }
}