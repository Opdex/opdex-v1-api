using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution
{
    public class SelectTokenDistributionByTokenIdQueryHandler : IRequestHandler<SelectTokenDistributionByTokenIdQuery, TokenDistribution>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(TokenDistributionEntity.Id)},
                {nameof(TokenDistributionEntity.TokenId)},
                {nameof(TokenDistributionEntity.MiningGovernanceId)},
                {nameof(TokenDistributionEntity.Owner)},
                {nameof(TokenDistributionEntity.Genesis)},
                {nameof(TokenDistributionEntity.PeriodDuration)},
                {nameof(TokenDistributionEntity.PeriodIndex)}
            FROM token_distribution
            WHERE {nameof(TokenDistributionEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectTokenDistributionByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<TokenDistribution> Handle(SelectTokenDistributionByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<TokenDistributionEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(TokenDistributionEntity)} with tokenId {request.TokenId} was not found.");
            }

            return _mapper.Map<TokenDistribution>(result);
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