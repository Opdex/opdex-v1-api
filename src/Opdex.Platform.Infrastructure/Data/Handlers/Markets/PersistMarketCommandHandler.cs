using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets;

public class PersistMarketCommandHandler : IRequestHandler<PersistMarketCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market (
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
              ) VALUES (
                @{nameof(MarketEntity.Address)},
                @{nameof(MarketEntity.DeployerId)},
                @{nameof(MarketEntity.StakingTokenId)},
                @{nameof(MarketEntity.PendingOwner)},
                @{nameof(MarketEntity.Owner)},
                @{nameof(MarketEntity.AuthPoolCreators)},
                @{nameof(MarketEntity.AuthProviders)},
                @{nameof(MarketEntity.AuthTraders)},
                @{nameof(MarketEntity.TransactionFee)},
                @{nameof(MarketEntity.MarketFeeEnabled)},
                @{nameof(MarketEntity.CreatedBlock)},
                @{nameof(MarketEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE market
                SET
                    {nameof(MarketEntity.PendingOwner)} = @{nameof(MarketEntity.PendingOwner)},
                    {nameof(MarketEntity.Owner)} = @{nameof(MarketEntity.Owner)},
                    {nameof(MarketEntity.ModifiedBlock)} = @{nameof(MarketEntity.ModifiedBlock)}
                WHERE {nameof(MarketEntity.Id)} = @{nameof(MarketEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistMarketCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistMarketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MarketEntity>(request.Market);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Contract", request.Market.Address },
                { "DeployerId", request.Market.DeployerId },
                { "BlockHeight", request.Market.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Failure persisting market.");
            }

            return 0;
        }
    }
}