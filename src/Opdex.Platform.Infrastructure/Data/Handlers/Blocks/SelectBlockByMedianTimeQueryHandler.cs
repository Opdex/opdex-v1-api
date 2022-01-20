using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks;

public class SelectBlockByMedianTimeQueryHandler : IRequestHandler<SelectBlockByMedianTimeQuery, Block>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.MedianTime)},
                {nameof(BlockEntity.Time)}
            FROM block
            WHERE {nameof(BlockEntity.MedianTime)} <= @{nameof(SqlParams.MedianTime)}
            ORDER BY {nameof(BlockEntity.Height)} DESC
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectBlockByMedianTimeQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Block> Handle(SelectBlockByMedianTimeQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.MedianTime), cancellationToken);

        var result = await _context.ExecuteFindAsync<BlockEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Block)} not found.");
        }

        return result == null ? null : _mapper.Map<Block>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(DateTime medianTime)
        {
            MedianTime = medianTime;
        }

        public DateTime MedianTime { get; }
    }
}