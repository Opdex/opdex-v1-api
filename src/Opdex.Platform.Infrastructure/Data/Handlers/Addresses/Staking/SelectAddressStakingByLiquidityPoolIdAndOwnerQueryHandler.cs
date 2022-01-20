using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;

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
            WHERE {nameof(AddressStakingEntity.Owner)} = @{nameof(SqlParams.Owner)}
                AND {nameof(AddressStakingEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
            LIMIT 1;".RemoveExcessWhitespace();

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

        var result = await _context.ExecuteFindAsync<AddressStakingEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"Staking position not found.");
        }

        return result == null ? null : _mapper.Map<AddressStaking>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong liquidityPoolId, Address owner)
        {
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
        }

        public ulong LiquidityPoolId { get; }
        public Address Owner { get; }
    }
}
