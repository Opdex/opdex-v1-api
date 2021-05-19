using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressBalanceByTokenIdAndOwnerQueryHandler
        : IRequestHandler<SelectAddressBalanceByTokenIdAndOwnerQuery, AddressBalance>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(AddressBalance.Id)},
                {nameof(AddressBalance.TokenId)},
                {nameof(AddressBalance.LiquidityPoolId)},
                {nameof(AddressBalance.Owner)},
                {nameof(AddressBalance.Balance)},
                {nameof(AddressBalance.CreatedBlock)},
                {nameof(AddressBalance.ModifiedBlock)}
            FROM address_balance
            WHERE {nameof(AddressBalance.TokenId)} = @{nameof(SqlParams.TokenId)}
                AND {nameof(AddressBalance.Owner)} = {nameof(SqlParams.Owner)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressBalanceByTokenIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressBalance> Handle(SelectAddressBalanceByTokenIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.Owner);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressBalance>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(AddressBalance)} not found.");
            }

            return result == null ? null : _mapper.Map<AddressBalance>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long liquidityPoolId, string owner)
            {
                TokenId = liquidityPoolId;
                Owner = owner;
            }

            public long TokenId { get; }
            public string Owner { get; }
        }
    }
}