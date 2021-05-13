using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectTokenByIdQueryHandler: IRequestHandler<SelectTokenByIdQuery, Token>
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
                {nameof(TokenEntity.CreatedDate)}
            FROM token
            WHERE {nameof(TokenEntity.Id)} = @{nameof(SqlParams.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokenByIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Token> Handle(SelectTokenByIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<TokenEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(TokenEntity)} with Id {request.TokenId} was not found.");
            }

            return _mapper.Map<Token>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long id)
            {
                Id = id;
            }

            public long Id { get; }
        }
    }
}