using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks
{
    public class SelectBlockByHeightQueryHandler : IRequestHandler<SelectBlockByHeightQuery, Block>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.MedianTime)},
                {nameof(BlockEntity.Time)}
            FROM block
            WHERE {nameof(BlockEntity.Height)} = @{nameof(BlockEntity.Height)}
            LIMIT 1;";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectBlockByHeightQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Block> Handle(SelectBlockByHeightQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new { request.Height }, token: cancellationToken);
            
            var result = await _context.ExecuteFindAsync<BlockEntity>(query);

            if (result == null)
            {
                throw new NotFoundException("No block found at height ${request.Height}.");
            }

            return _mapper.Map<Block>(result);
        }
    }
}