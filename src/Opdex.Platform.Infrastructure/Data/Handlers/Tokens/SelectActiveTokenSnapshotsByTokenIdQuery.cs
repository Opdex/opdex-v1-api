using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectActiveTokenSnapshotsByTokenIdQueryHandler : IRequestHandler<SelectActiveTokenSnapshotsByTokenIdQuery, IEnumerable<TokenSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Price)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)}
            FROM token_snapshot
            WHERE {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
                AND @{nameof(SqlParams.Time)} BETWEEN 
                    {nameof(TokenSnapshotEntity.StartDate)} AND {nameof(TokenSnapshotEntity.EndDate)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectActiveTokenSnapshotsByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenSnapshot>> Handle(SelectActiveTokenSnapshotsByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.Time);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<TokenSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<TokenSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, DateTime time)
            {
                TokenId = tokenId;
                Time = time;
            }

            public long TokenId { get; }
            public DateTime Time { get; }
        }
    }
}