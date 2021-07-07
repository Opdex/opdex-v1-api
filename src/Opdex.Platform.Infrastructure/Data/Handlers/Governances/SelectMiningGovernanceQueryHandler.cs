using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Governances
{
    public class SelectMiningGovernanceQueryHandler : IRequestHandler<SelectMiningGovernanceQuery, MiningGovernance>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(MiningGovernanceEntity.Id)},
                {nameof(MiningGovernanceEntity.Address)},
                {nameof(MiningGovernanceEntity.TokenId)},
                {nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                {nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                {nameof(MiningGovernanceEntity.MiningPoolReward)},
                {nameof(MiningGovernanceEntity.CreatedBlock)},
                {nameof(MiningGovernanceEntity.ModifiedBlock)}
            FROM governance
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMiningGovernanceQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MiningGovernance> Handle(SelectMiningGovernanceQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, cancellationToken);

            var result = await _context.ExecuteFindAsync<MiningGovernanceEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(MiningGovernance)} not found.");
            }

            return result == null ? null : _mapper.Map<MiningGovernance>(result);
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
