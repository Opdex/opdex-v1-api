using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Wrapped;

public class SelectTokenWrappedByTokenIdQueryHandler : IRequestHandler<SelectTokenWrappedByTokenIdQuery, TokenWrapped>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenWrappedEntity.Id)},
                {nameof(TokenWrappedEntity.TokenId)},
                {nameof(TokenWrappedEntity.Owner)},
                {nameof(TokenWrappedEntity.NativeChainTypeId)},
                {nameof(TokenWrappedEntity.NativeAddress)},
                {nameof(TokenWrappedEntity.Validated)},
                {nameof(TokenWrappedEntity.Trusted)},
                {nameof(TokenWrappedEntity.CreatedBlock)},
                {nameof(TokenWrappedEntity.ModifiedBlock)}
            FROM token_wrapped
            WHERE {nameof(TokenWrappedEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenWrappedByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenWrapped> Handle(SelectTokenWrappedByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.TokenId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenWrappedEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Wrapped token not found.");
        }

        return result is null ? null : _mapper.Map<TokenWrapped>(result);
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
