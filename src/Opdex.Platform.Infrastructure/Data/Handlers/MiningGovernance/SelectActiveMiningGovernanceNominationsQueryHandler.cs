using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance
{
    public class SelectActiveMiningGovernanceNominationsQueryHandler 
        : IRequestHandler<SelectActiveMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNomination>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(MiningGovernanceNominationEntity.Id)},
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
            FROM odx_mining_governance_nomination
            WHERE 
                {nameof(MiningGovernanceNominationEntity.IsNominated)} = true;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectActiveMiningGovernanceNominationsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MiningGovernanceNomination>> Handle(SelectActiveMiningGovernanceNominationsQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, cancellationToken);

            var result = await _context.ExecuteQueryAsync<MiningGovernanceNominationEntity>(query);

            return _mapper.Map<IEnumerable<MiningGovernanceNomination>>(result);
        }
    }
}