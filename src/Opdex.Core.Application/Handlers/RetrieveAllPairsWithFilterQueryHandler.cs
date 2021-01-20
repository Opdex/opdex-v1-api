using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application.Handlers
{
    public class RetrieveAllPairsWithFilterQueryHandler : IRequestHandler<RetrieveAllPairsWithFilterQuery, IEnumerable<PairDto>>
    {
        public Task<IEnumerable<PairDto>> Handle(RetrieveAllPairsWithFilterQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}