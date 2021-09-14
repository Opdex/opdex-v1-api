using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances
{
    public class SelectAddressBalancesByModifiedBlockQueryHandler : IRequestHandler<SelectAddressBalancesByModifiedBlockQuery, IEnumerable<AddressBalance>>
    {
        private static readonly string SqlQuery =
            @$"Select
                {nameof(AddressMiningEntity.Id)},
                {nameof(AddressMiningEntity.MiningPoolId)},
                {nameof(AddressMiningEntity.Owner)},
                {nameof(AddressMiningEntity.Balance)},
                {nameof(AddressMiningEntity.CreatedBlock)},
                {nameof(AddressMiningEntity.ModifiedBlock)}
            FROM address_balance
            WHERE {nameof(AddressMiningEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressBalancesByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressBalance>> Handle(SelectAddressBalancesByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressMiningEntity>(query);

            return _mapper.Map<IEnumerable<AddressBalance>>(result);
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
