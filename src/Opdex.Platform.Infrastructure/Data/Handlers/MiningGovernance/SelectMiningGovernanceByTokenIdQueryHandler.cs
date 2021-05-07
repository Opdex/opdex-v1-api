using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance
{
    public class SelectMiningGovernanceByTokenIdQueryHandler : IRequestHandler<SelectMiningGovernanceByTokenIdQuery, Domain.MiningGovernance>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(MiningGovernanceEntity.Id)},
                {nameof(MiningGovernanceEntity.Address)},
                {nameof(MiningGovernanceEntity.TokenId)},
                {nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                {nameof(MiningGovernanceEntity.Balance)},
                {nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                {nameof(MiningGovernanceEntity.MiningPoolReward)}
            FROM mining_governance
            WHERE {nameof(MiningGovernanceEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectMiningGovernanceByTokenIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Domain.MiningGovernance> Handle(SelectMiningGovernanceByTokenIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<MiningGovernanceEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(MiningGovernanceEntity)} with tokenId {request.TokenId} was not found.");
            }

            return _mapper.Map<Domain.MiningGovernance>(result);
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