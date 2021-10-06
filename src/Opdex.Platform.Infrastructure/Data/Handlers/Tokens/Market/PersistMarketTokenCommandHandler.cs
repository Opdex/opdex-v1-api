using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Market
{
    public class PersistMarketTokenCommandHandler : IRequestHandler<PersistMarketTokenCommand, bool>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT IGNORE INTO market_token (
                {nameof(MarketTokenEntity.MarketId)},
                {nameof(MarketTokenEntity.TokenId)},
                {nameof(MarketTokenEntity.CreatedBlock)},
                {nameof(MarketTokenEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MarketTokenEntity.MarketId)},
                @{nameof(MarketTokenEntity.TokenId)},
                @{nameof(MarketTokenEntity.CreatedBlock)},
                @{nameof(MarketTokenEntity.ModifiedBlock)}
              );";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistMarketTokenCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketTokenCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(PersistMarketTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<MarketTokenEntity>(request.MarketToken);

                var command = DatabaseQuery.Create(InsertSqlCommand, entity, cancellationToken);

                var result = await _context.ExecuteCommandAsync(command);

                return result >= 1;
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["MarketId"] = request.MarketToken.MarketId,
                    ["TokenId"] = request.MarketToken.TokenId
                }))
                {
                    _logger.LogError(ex, "Failure persisting market token.");
                }

                throw;
            }
        }
    }
}
