using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetAllPoolsQuery : IRequest<IEnumerable<LiquidityPoolDto>>
    {
        
    }
}