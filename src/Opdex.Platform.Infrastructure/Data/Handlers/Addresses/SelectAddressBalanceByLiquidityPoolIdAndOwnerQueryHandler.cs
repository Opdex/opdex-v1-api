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
    public class SelectAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler 
        : IRequestHandler<SelectAddressBalanceByLiquidityPoolIdAndOwnerQuery, AddressBalance>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(AddressBalanceEntity.Id)},
                {nameof(AddressBalanceEntity.TokenId)},
                {nameof(AddressBalanceEntity.LiquidityPoolId)},
                {nameof(AddressBalanceEntity.Owner)},
                {nameof(AddressBalanceEntity.Balance)},
                {nameof(AddressBalanceEntity.CreatedBlock)},
                {nameof(AddressBalanceEntity.ModifiedBlock)}
            FROM address_balance
            WHERE {nameof(AddressBalanceEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
                AND {nameof(AddressBalanceEntity.Owner)} = {nameof(SqlParams.Owner)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressBalance> Handle(SelectAddressBalanceByLiquidityPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.LiquidityPoolId, request.Owner);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressBalance>(query);

            return _mapper.Map<AddressBalance>(result);
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