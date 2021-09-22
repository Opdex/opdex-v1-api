using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectMarketsByModifiedBlockQueryHandler : IRequestHandler<SelectMarketsByModifiedBlockQuery, IEnumerable<Market>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(MarketEntity.Id)},
                {nameof(MarketEntity.Address)},
                {nameof(MarketEntity.DeployerId)},
                {nameof(MarketEntity.StakingTokenId)},
                {nameof(MarketEntity.Owner)},
                {nameof(MarketEntity.AuthPoolCreators)},
                {nameof(MarketEntity.AuthProviders)},
                {nameof(MarketEntity.AuthTraders)},
                {nameof(MarketEntity.TransactionFee)},
                {nameof(MarketEntity.MarketFeeEnabled)},
                {nameof(MarketEntity.CreatedBlock)},
                {nameof(MarketEntity.ModifiedBlock)}
            FROM market
            WHERE {nameof(MarketEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMarketsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Market>> Handle(SelectMarketsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<MarketEntity>(query);

            return _mapper.Map<IEnumerable<Market>>(result);
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
