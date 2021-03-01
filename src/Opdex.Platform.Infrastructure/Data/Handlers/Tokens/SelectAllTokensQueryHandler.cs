using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectAllTokensQueryHandler : IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(TokenEntity.Id)},
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.TotalSupply)},
                {nameof(TokenEntity.CreatedDate)}
            FROM token;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectAllTokensQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectAllTokensQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<Token>> Handle(SelectAllTokensQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, null, cancellationToken);
            
            var tokenEntities =  await _context.ExecuteQueryAsync<TokenEntity>(command);

            return _mapper.Map<IEnumerable<Token>>(tokenEntities);
        }
    }
}