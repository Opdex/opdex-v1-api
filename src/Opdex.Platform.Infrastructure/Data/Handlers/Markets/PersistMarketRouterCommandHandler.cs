using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets;

public class PersistMarketRouterCommandHandler : IRequestHandler<PersistMarketRouterCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market_router (
                {nameof(MarketRouterEntity.Address)},
                {nameof(MarketRouterEntity.MarketId)},
                {nameof(MarketRouterEntity.IsActive)},
                {nameof(MarketRouterEntity.CreatedBlock)},
                {nameof(MarketRouterEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MarketRouterEntity.Address)},
                @{nameof(MarketRouterEntity.MarketId)},
                @{nameof(MarketRouterEntity.IsActive)},
                @{nameof(MarketRouterEntity.CreatedBlock)},
                @{nameof(MarketRouterEntity.ModifiedBlock)}
              );";
        
    private static readonly string UpdateSqlCommand =
        $@"UPDATE market_router
                SET 
                    {nameof(MarketRouterEntity.IsActive)} = @{nameof(MarketRouterEntity.IsActive)},
                    {nameof(MarketRouterEntity.ModifiedBlock)} = @{nameof(MarketRouterEntity.ModifiedBlock)}
                WHERE {nameof(MarketRouterEntity.Id)} = @{nameof(MarketRouterEntity.Id)};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
        
    public PersistMarketRouterCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketRouterCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistMarketRouterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MarketRouterEntity>(request.Router);
                
            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;
            
            var command = DatabaseQuery.Create(sql, entity, cancellationToken);
            
            var result = await _context.ExecuteCommandAsync(command);

            return result == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure persisting {request.Router}.");
                
            return false;
        }
    }
}