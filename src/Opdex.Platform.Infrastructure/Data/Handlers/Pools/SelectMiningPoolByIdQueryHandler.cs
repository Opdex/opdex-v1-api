using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectMiningPoolByIdQueryHandler : IRequestHandler<SelectMiningPoolByIdQuery, MiningPool>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(MiningPoolEntity.Id)},
                {nameof(MiningPoolEntity.LiquidityPoolId)},
                {nameof(MiningPoolEntity.Address)},
                {nameof(MiningPoolEntity.RewardPerBlock)},
                {nameof(MiningPoolEntity.RewardPerLpt)},
                {nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                {nameof(MiningPoolEntity.ModifiedBlock)},
                {nameof(MiningPoolEntity.CreatedBlock)}
            FROM pool_mining
            WHERE {nameof(MiningPoolEntity.Id)} = @{nameof(SqlParams.Id)} LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMiningPoolByIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MiningPool> Handle(SelectMiningPoolByIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MiningPoolId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<MiningPoolEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(MiningPool)} not found.");
            }

            return result == null ? null : _mapper.Map<MiningPool>(result);
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