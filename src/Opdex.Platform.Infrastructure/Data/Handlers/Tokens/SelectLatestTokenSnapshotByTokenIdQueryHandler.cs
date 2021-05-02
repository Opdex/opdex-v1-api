using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectLatestTokenSnapshotByTokenIdQueryHandler : IRequestHandler<SelectLatestTokenSnapshotByTokenIdQuery, TokenSnapshot>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.Price)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotType)}
            FROM token_snapshot
            WHERE {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            ORDER BY {nameof(TokenSnapshotEntity.EndDate)}, {nameof(TokenSnapshotEntity.SnapshotType)} DESC
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLatestTokenSnapshotByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TokenSnapshot> Handle(SelectLatestTokenSnapshotByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<TokenSnapshotEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"Token snapshot by id {request.TokenId} not found.");
            }
            return _mapper.Map<TokenSnapshot>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId)
            {
                TokenId = tokenId;
            }

            public long TokenId { get; }
        }
    }
}