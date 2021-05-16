using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressStakingByLiquidityPoolIdAndOwnerQueryHandler
        : IRequestHandler<SelectAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(AddressStakingEntity.Id)},
                {nameof(AddressStakingEntity.LiquidityPoolId)},
                {nameof(AddressStakingEntity.Owner)},
                {nameof(AddressStakingEntity.Weight)},
                {nameof(AddressStakingEntity.CreatedBlock)},
                {nameof(AddressStakingEntity.ModifiedBlock)}
            FROM address_staking
            WHERE {nameof(AddressStakingEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
                AND {nameof(AddressStakingEntity.Owner)} = {nameof(SqlParams.Owner)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressStakingByLiquidityPoolIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressStaking> Handle(SelectAddressStakingByLiquidityPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.LiquidityPoolId, request.Owner);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressStakingEntity>(query);

            return _mapper.Map<AddressStaking>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long liquidityPoolId, string owner)
            {
                LiquidityPoolId = liquidityPoolId;
                Owner = owner;
            }

            public long LiquidityPoolId { get; }
            public string Owner { get; }
        }
    }
}