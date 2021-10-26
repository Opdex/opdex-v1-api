using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Attributes
{
    public class SelectTokenAttributesByTokenIdQueryHandler
        : IRequestHandler<SelectTokenAttributesByTokenIdQuery, IEnumerable<TokenAttribute>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(TokenAttributeEntity.Id)},
                {nameof(TokenAttributeEntity.TokenId)},
                {nameof(TokenAttributeEntity.AttributeTypeId)}
            FROM token_attribute
            WHERE {nameof(TokenAttributeEntity.TokenId)} = @{nameof(SqlParams.TokenId)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokenAttributesByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenAttribute>> Handle(SelectTokenAttributesByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

            var result = await _context.ExecuteQueryAsync<TokenAttributeEntity>(query);

            return _mapper.Map<IEnumerable<TokenAttribute>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong tokenId)
            {
                TokenId = tokenId;
            }

            public ulong TokenId { get; }
        }
    }
}
