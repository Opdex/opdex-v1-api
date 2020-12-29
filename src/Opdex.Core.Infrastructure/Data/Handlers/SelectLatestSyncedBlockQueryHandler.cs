using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.BasePlatform.Infrastructure.Data.Handlers
{
    public class SelectLatestSyncedBlockQueryHandler: IRequestHandler<SelectLatestSyncedBlockQuery, Block>
    {
        private static readonly string SqlQuery = $@"";
        private readonly IDbContext _context;

        public SelectLatestSyncedBlockQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Block> Handle(SelectLatestSyncedBlockQuery request, CancellationToken cancellationToken)
        {
            return new Block();
        }
    }
}