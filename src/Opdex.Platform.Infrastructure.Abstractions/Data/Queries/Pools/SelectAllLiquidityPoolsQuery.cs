using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectAllLiquidityPoolsQuery : IRequest<IEnumerable<LiquidityPool>>
    {
        
    }
}