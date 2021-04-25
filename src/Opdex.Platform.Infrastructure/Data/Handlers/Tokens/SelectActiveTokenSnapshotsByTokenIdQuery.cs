using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    // Todo: This isn't designed particularly well for resyncs or scanning historical transactions
    public class SelectActiveTokenSnapshotsByTokenIdQueryHandler : IRequestHandler<SelectActiveTokenSnapshotsByTokenIdQuery, IEnumerable<TokenSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.Price)},
                {nameof(TokenSnapshotEntity.SnapshotStartDate)},
                {nameof(TokenSnapshotEntity.SnapshotEndDate)},
                {nameof(TokenSnapshotEntity.SnapshotType)}
            FROM token_snapshot
            WHERE {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
                AND {nameof(TokenSnapshotEntity.SnapshotEndDate)} >= @{nameof(SqlParams.Now)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectActiveTokenSnapshotsByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenSnapshot>> Handle(SelectActiveTokenSnapshotsByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.Now);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<TokenSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<TokenSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, DateTime now)
            {
                TokenId = tokenId;
                Now = now;
            }

            public long TokenId { get; }
            public DateTime Now { get; }
        }
    }
}