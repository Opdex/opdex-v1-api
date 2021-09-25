using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking
{
    public class SelectStakingPositionsByModifiedBlockQueryHandler : IRequestHandler<SelectStakingPositionsByModifiedBlockQuery, IEnumerable<AddressStaking>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(AddressStakingEntity.Id)},
                {nameof(AddressStakingEntity.LiquidityPoolId)},
                {nameof(AddressStakingEntity.Owner)},
                {nameof(AddressStakingEntity.Weight)},
                {nameof(AddressStakingEntity.CreatedBlock)},
                {nameof(AddressStakingEntity.ModifiedBlock)}
            FROM address_staking
            WHERE {nameof(AddressStakingEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectStakingPositionsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressStaking>> Handle(SelectStakingPositionsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressStakingEntity>(query);

            return _mapper.Map<IEnumerable<AddressStaking>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong modifiedBlock)
            {
                ModifiedBlock = modifiedBlock;
            }

            public ulong ModifiedBlock { get; }
        }
    }
}
