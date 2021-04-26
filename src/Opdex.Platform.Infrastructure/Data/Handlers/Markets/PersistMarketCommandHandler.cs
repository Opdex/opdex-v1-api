using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class PersistMarketCommandHandler: IRequestHandler<PersistMarketCommand, long>
    {
        // Todo: Insert vs update
        private static readonly string SqlCommand =
            $@"INSERT INTO pool_liquidity (
                {nameof(MarketEntity.Address)},
                {nameof(MarketEntity.AuthPoolCreators)},
                {nameof(MarketEntity.AuthProviders)},
                {nameof(MarketEntity.AuthTraders)},
                {nameof(MarketEntity.Fee)},
                {nameof(MarketEntity.Staking)},
                {nameof(MarketEntity.CreatedDate)}
              ) VALUES (
                @{nameof(MarketEntity.Address)},
                @{nameof(MarketEntity.AuthPoolCreators)},
                @{nameof(MarketEntity.AuthProviders)},
                @{nameof(MarketEntity.AuthTraders)},
                @{nameof(MarketEntity.Fee)},
                @{nameof(MarketEntity.Staking)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistMarketCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistMarketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var market = _mapper.Map<MarketEntity>(request.Market);
            
                var command = DatabaseQuery.Create(SqlCommand, market, cancellationToken);
            
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.Market}");
                
                return 0;
            }
        }
    }
}