using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;

public class PersistTokenDistributionCommandHandler : IRequestHandler<PersistTokenDistributionCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_distribution (
                {nameof(TokenDistributionEntity.TokenId)},
                {nameof(TokenDistributionEntity.VaultDistribution)},
                {nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                {nameof(TokenDistributionEntity.PeriodIndex)},
                {nameof(TokenDistributionEntity.DistributionBlock)},
                {nameof(TokenDistributionEntity.NextDistributionBlock)},
                {nameof(TokenDistributionEntity.CreatedBlock)},
                {nameof(TokenDistributionEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(TokenDistributionEntity.TokenId)},
                @{nameof(TokenDistributionEntity.VaultDistribution)},
                @{nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                @{nameof(TokenDistributionEntity.PeriodIndex)},
                @{nameof(TokenDistributionEntity.DistributionBlock)},
                @{nameof(TokenDistributionEntity.NextDistributionBlock)},
                @{nameof(TokenDistributionEntity.CreatedBlock)},
                @{nameof(TokenDistributionEntity.ModifiedBlock)}
              );";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenDistributionCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenDistributionCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistTokenDistributionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var poolEntity = _mapper.Map<TokenDistributionEntity>(request.TokenDistribution);

            var command = DatabaseQuery.Create(InsertSqlCommand, poolEntity, cancellationToken);

            var result = await _context.ExecuteCommandAsync(command);

            return result >= 1;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "TokenId", request.TokenDistribution.TokenId },
                { "PeriodIndex", request.TokenDistribution.PeriodIndex },
                { "BlockHeight", request.TokenDistribution.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Failure persisting token distribution.");
            }

            return false;
        }
    }
}