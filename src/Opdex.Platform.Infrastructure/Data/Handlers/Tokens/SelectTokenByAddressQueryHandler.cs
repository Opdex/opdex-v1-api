using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens;

public class SelectTokenByAddressQueryHandler : IRequestHandler<SelectTokenByAddressQuery, Token>
{
    private static readonly string SqlQuery =
        @$"Select
                {nameof(TokenEntity.Id)},
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.TotalSupply)},
                {nameof(TokenEntity.CreatedBlock)},
                {nameof(TokenEntity.ModifiedBlock)}
            FROM token
            WHERE {nameof(TokenEntity.Address)} = @{nameof(SqlParams.Address)}
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenByAddressQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Token> Handle(SelectTokenByAddressQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.Address);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Token)} not found.");
        }

        return result == null ? null : _mapper.Map<Token>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }
}
