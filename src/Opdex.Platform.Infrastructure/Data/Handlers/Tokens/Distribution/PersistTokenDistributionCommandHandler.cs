using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution
{
    public class PersistTokenDistributionCommandHandler : IRequestHandler<PersistTokenDistributionCommand, bool>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO odx_token_distribution (
                {nameof(TokenDistributionEntity.VaultDistribution)},
                {nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                {nameof(TokenDistributionEntity.PeriodIndex)},
                {nameof(TokenDistributionEntity.DistributionBlock)},
                {nameof(TokenDistributionEntity.NextDistributionBlock)}
              ) VALUES (
                @{nameof(TokenDistributionEntity.Id)},
                @{nameof(TokenDistributionEntity.VaultDistribution)},
                @{nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                @{nameof(TokenDistributionEntity.PeriodIndex)},
                @{nameof(TokenDistributionEntity.DistributionBlock)},
                @{nameof(TokenDistributionEntity.NextDistributionBlock)}
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
                _logger.LogError(ex, $"Failure persisting {nameof(TokenDistributionEntity)} record.");
                
                return false;
            }
        }
    }
}