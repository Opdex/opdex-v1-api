using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution
{
    public class SelectLatestTokenDistributionQueryHandler : IRequestHandler<SelectLatestTokenDistributionQuery, TokenDistribution>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(TokenDistributionEntity.Id)},
                {nameof(TokenDistributionEntity.VaultDistribution)},
                {nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                {nameof(TokenDistributionEntity.PeriodIndex)},
                {nameof(TokenDistributionEntity.DistributionBlock)},
                {nameof(TokenDistributionEntity.NextDistributionBlock)},
                {nameof(TokenDistributionEntity.CreatedBlock)},
                {nameof(TokenDistributionEntity.ModifiedBlock)}
            FROM odx_token_distribution
            ORDER BY {nameof(TokenDistributionEntity.NextDistributionBlock)} DESC
            LIMIT 1;";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectLatestTokenDistributionQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<TokenDistribution> Handle(SelectLatestTokenDistributionQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<TokenDistributionEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(TokenDistribution)} not found.");
            }

            return result == null ? null : _mapper.Map<TokenDistribution>(result);
        }
    }
}