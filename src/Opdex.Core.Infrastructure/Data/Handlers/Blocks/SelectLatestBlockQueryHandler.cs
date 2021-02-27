using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Core.Infrastructure.Data.Handlers.Blocks
{
    public class SelectLatestBlockQueryHandler: IRequestHandler<SelectLatestBlockQuery, Block>
    {
        private static readonly string SqlQuery = 
            @$"Select 
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.MedianTime)},
                {nameof(BlockEntity.Time)}
            FROM block
            ORDER BY {nameof(BlockEntity.Height)} DESC
            LIMIT 1;";
                        
        private readonly IDbContext _context;

        public SelectLatestBlockQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Block> Handle(SelectLatestBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, token: cancellationToken);
            
            var result = await _context.ExecuteFindAsync<BlockEntity>(query);

            if (result == null)
            {
                throw new NotFoundException("No blocks found.");
            }
            
            return new Block(result.Height, result.Hash, result.Time, result.Time);
        }
    }
}