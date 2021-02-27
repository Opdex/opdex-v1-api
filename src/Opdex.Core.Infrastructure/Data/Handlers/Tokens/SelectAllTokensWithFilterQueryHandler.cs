using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Core.Infrastructure.Data.Handlers.Tokens
{
    public class SelectAllTokensWithFilterQueryHandler : IRequestHandler<SelectAllTokensWithFilterQuery, IEnumerable<Token>>
    {
        private static readonly string SqlQuery = $@"";
        private readonly IDbContext _context;

        public SelectAllTokensWithFilterQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<IEnumerable<Token>> Handle(SelectAllTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<Token>());
        }
    }
}