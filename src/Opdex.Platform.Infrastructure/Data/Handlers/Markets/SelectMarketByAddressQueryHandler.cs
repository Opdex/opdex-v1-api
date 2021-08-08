using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectMarketByAddressQueryHandler : IRequestHandler<SelectMarketByAddressQuery, Market>
    {
        private static readonly string SqlCommand =
            $@"SELECT
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
            WHERE {nameof(MarketEntity.Address)} = @{nameof(SqlParams.Address)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectMarketByAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Market> Handle(SelectMarketByAddressQuery request, CancellationToken cancellationToken)
        {
            var sqlParams = new SqlParams(request.Address);
            
            var command = DatabaseQuery.Create(SqlCommand, sqlParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<MarketEntity>(command);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(Market)} not found.");
            }

            return result == null ? null : _mapper.Map<Market>(result);
        }
        
        private sealed class SqlParams
        {
            internal SqlParams(string address)
            {
                Address = address;
            }

            public string Address { get; }
        }
    }
}