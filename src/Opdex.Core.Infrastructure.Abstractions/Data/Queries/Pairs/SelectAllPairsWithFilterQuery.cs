using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs
{
    public class SelectAllPairsWithFilterQuery : IRequest<IEnumerable<Pair>>
    {
        public SelectAllPairsWithFilterQuery()
        {
            
        }
    }
}