using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Core.Infrastructure.Data.Handlers.Tokens
{
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
                {nameof(TokenEntity.CreatedDate)}
            FROM token
            WHERE {nameof(TokenEntity.Address)} = @{nameof(SqlParams.Address)};";

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

            if (result == null)
            {
                throw new NotFoundException($"{nameof(TokenEntity)} with address {request.Address} was not found.");
            }

            return _mapper.Map<Token>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string address)
            {
                Address = address;
            }

            public string Address { get; }
        }
    }
}