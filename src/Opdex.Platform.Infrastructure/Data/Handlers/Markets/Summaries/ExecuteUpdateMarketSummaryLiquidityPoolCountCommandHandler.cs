using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;

public class ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler : IRequestHandler<ExecuteUpdateMarketSummaryLiquidityPoolCountCommand, bool>
{
    private const string SqlCommand = "UpdateMarketSummaryLiquidityPoolCount";

    private readonly IDbContext _context;
    private readonly ILogger _logger;

    public ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler(IDbContext context, ILogger<ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ExecuteUpdateMarketSummaryLiquidityPoolCountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = DatabaseQuery.Create(SqlCommand, CommandType.StoredProcedure,new { marketId = request.MarketId, blockHeight = request.BlockHeight }, cancellationToken);

            await _context.ExecuteCommandAsync(command);

            return true;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
               { nameof(request.MarketId), request.MarketId },
               { nameof(request.BlockHeight), request.BlockHeight }
            }))
            {
                _logger.LogError(ex, "Failed to update market summary liquidity pool count");
                return false;
            }
        }
    }
}
