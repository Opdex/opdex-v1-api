using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining
{
    public class SelectMiningPositionsByModifiedBlockQueryHandler : IRequestHandler<SelectMiningPositionsByModifiedBlockQuery, IEnumerable<AddressMining>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(AddressMiningEntity.Id)},
                {nameof(AddressMiningEntity.MiningPoolId)},
                {nameof(AddressMiningEntity.Owner)},
                {nameof(AddressMiningEntity.Balance)},
                {nameof(AddressMiningEntity.CreatedBlock)},
                {nameof(AddressMiningEntity.ModifiedBlock)}
            FROM address_mining
            WHERE {nameof(AddressMiningEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMiningPositionsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressMining>> Handle(SelectMiningPositionsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressMiningEntity>(query);

            return _mapper.Map<IEnumerable<AddressMining>>(result);
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
