using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectAllMarketsQueryHandler : IRequestHandler<SelectAllMarketsQuery, IEnumerable<Market>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(MarketEntity.Id)},
                {nameof(MarketEntity.Address)},
                {nameof(MarketEntity.DeployerId)},
                {nameof(MarketEntity.StakingTokenId)},
                {nameof(MarketEntity.PendingOwner)},
                {nameof(MarketEntity.Owner)},
                {nameof(MarketEntity.AuthPoolCreators)},
                {nameof(MarketEntity.AuthProviders)},
                {nameof(MarketEntity.AuthTraders)},
                {nameof(MarketEntity.TransactionFee)},
                {nameof(MarketEntity.MarketFeeEnabled)},
                {nameof(MarketEntity.CreatedBlock)},
                {nameof(MarketEntity.ModifiedBlock)}
            FROM market;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAllMarketsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Market>> Handle(SelectAllMarketsQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, cancellationToken);

            var result = await _context.ExecuteQueryAsync<MarketEntity>(command);

            return _mapper.Map<IEnumerable<Market>>(result);
        }
    }
}
