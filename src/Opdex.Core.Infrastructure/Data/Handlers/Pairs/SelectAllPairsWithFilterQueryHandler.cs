using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Core.Infrastructure.Data.Handlers.Pairs
{
    public class SelectAllPairsWithFilterQueryHandler : IRequestHandler<SelectAllPairsWithFilterQuery, IEnumerable<Pair>>
    {
        private static readonly string SqlQuery = $@"";
        private readonly IDbContext _context;

        public SelectAllPairsWithFilterQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<IEnumerable<Pair>> Handle(SelectAllPairsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<Pair>());
        }
    }
}