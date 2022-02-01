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

public class SelectTokenChainByTokenIdQueryHandler : IRequestHandler<SelectTokenChainByTokenIdQuery, TokenChain>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenChainEntity.Id)},
                {nameof(TokenChainEntity.TokenId)},
                {nameof(TokenChainEntity.NativeChainTypeId)},
                {nameof(TokenChainEntity.NativeAddress)}
            FROM token_chain
            WHERE {nameof(TokenChainEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenChainByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenChain> Handle(SelectTokenChainByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.TokenId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenChainEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Token chain not found.");
        }

        return result is null ? null : _mapper.Map<TokenChain>(result);
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
