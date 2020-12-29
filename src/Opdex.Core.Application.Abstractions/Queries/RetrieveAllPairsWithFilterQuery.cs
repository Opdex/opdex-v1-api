using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Core.Application.Abstractions.Queries
{
    public class RetrieveAllPairsWithFilterQuery : IRequest<IEnumerable<PairDto>>
    {
        
    }
}