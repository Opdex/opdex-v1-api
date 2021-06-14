using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetAllPoolsQuery : IRequest<IEnumerable<LiquidityPoolDto>>
    {
        
    }
}