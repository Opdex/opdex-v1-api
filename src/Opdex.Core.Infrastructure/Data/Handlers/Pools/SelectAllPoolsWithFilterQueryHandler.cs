using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Core.Infrastructure.Data.Handlers.Pools
{
    public class SelectAllPoolsWithFilterQueryHandler : IRequestHandler<SelectAllPoolsWithFilterQuery, IEnumerable<Pool>>
    {
        private static readonly string SqlQuery = $@"";
        private readonly IDbContext _context;

        public SelectAllPoolsWithFilterQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<IEnumerable<Pool>> Handle(SelectAllPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<Pool>());
        }
    }
}