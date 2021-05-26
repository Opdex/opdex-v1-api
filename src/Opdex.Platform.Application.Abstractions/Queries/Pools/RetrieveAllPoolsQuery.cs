using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveAllPoolsQuery : IRequest<IEnumerable<LiquidityPool>>
    {
    }
}