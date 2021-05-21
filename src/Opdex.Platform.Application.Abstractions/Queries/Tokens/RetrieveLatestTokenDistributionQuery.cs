using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveLatestTokenDistributionQuery : FindQuery<TokenDistribution>
    {
        public RetrieveLatestTokenDistributionQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}