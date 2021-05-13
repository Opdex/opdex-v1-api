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
                {nameof(MarketEntity.Fee)},
                {nameof(MarketEntity.CreatedDate)}
            FROM market
            WHERE {nameof(MarketEntity.Address)} = @{nameof(MarketEntity.Address)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectMarketByAddressQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectMarketByAddressQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Market> Handle(SelectMarketByAddressQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, request, cancellationToken);
            
            var marketEntity =  await _context.ExecuteFindAsync<MarketEntity>(command);

            if (marketEntity == null)
            {
                throw new NotFoundException($"{nameof(Market)} not found with address {request.Address}");
            }

            return _mapper.Map<Market>(marketEntity);
        }
    }
}