using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;

public class SelectAddressBalanceByOwnerAndTokenIdQueryHandler
    : IRequestHandler<SelectAddressBalanceByOwnerAndTokenIdQuery, AddressBalance>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(AddressBalanceEntity.Id)},
                {nameof(AddressBalanceEntity.TokenId)},
                {nameof(AddressBalanceEntity.Owner)},
                {nameof(AddressBalanceEntity.Balance)},
                {nameof(AddressBalanceEntity.CreatedBlock)},
                {nameof(AddressBalanceEntity.ModifiedBlock)}
            FROM address_balance
            WHERE {nameof(AddressBalanceEntity.Owner)} = @{nameof(SqlParams.Owner)} AND
                {nameof(AddressBalanceEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectAddressBalanceByOwnerAndTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AddressBalance> Handle(SelectAddressBalanceByOwnerAndTokenIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.TokenId, request.Owner);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<AddressBalanceEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(AddressBalance)} not found.");
        }

        return result == null ? null : _mapper.Map<AddressBalance>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong tokenId, Address owner)
        {
            TokenId = tokenId;
            Owner = owner;
        }

        public ulong TokenId { get; }
        public Address Owner { get; }
    }
}