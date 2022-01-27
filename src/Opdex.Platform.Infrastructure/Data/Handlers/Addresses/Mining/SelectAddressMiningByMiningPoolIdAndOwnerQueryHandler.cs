using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;

public class SelectAddressMiningByMiningPoolIdAndOwnerQueryHandler
    : IRequestHandler<SelectAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>
{
    private static readonly string SqlQuery =
        @$"Select
                {nameof(AddressMiningEntity.Id)},
                {nameof(AddressMiningEntity.MiningPoolId)},
                {nameof(AddressMiningEntity.Owner)},
                {nameof(AddressMiningEntity.Balance)},
                {nameof(AddressMiningEntity.CreatedBlock)},
                {nameof(AddressMiningEntity.ModifiedBlock)}
            FROM address_mining
            WHERE {nameof(AddressMiningEntity.Owner)} = @{nameof(SqlParams.Owner)}
                AND {nameof(AddressMiningEntity.MiningPoolId)} = @{nameof(SqlParams.MiningPoolId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectAddressMiningByMiningPoolIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AddressMining> Handle(SelectAddressMiningByMiningPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.MiningPoolId, request.Owner);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<AddressMiningEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Mining position not found.");
        }

        return result == null ? null : _mapper.Map<AddressMining>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong miningPoolId, Address owner)
        {
            MiningPoolId = miningPoolId;
            Owner = owner;
        }

        public ulong MiningPoolId { get; }
        public Address Owner { get; }
    }
}
