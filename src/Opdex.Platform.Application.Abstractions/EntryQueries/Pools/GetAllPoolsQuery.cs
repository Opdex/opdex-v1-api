using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetAllPoolsQuery : IRequest<IEnumerable<PoolDto>>
    {
        
    }
}