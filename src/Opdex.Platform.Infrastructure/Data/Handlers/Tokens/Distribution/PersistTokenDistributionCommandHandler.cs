using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution
{
    public class PersistTokenDistributionCommandHandler : IRequestHandler<PersistTokenDistributionCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO token_distribution (
                {nameof(TokenDistributionEntity.TokenId)},
                {nameof(TokenDistributionEntity.MiningGovernanceId)},
                {nameof(TokenDistributionEntity.Owner)},
                {nameof(TokenDistributionEntity.Genesis)},
                {nameof(TokenDistributionEntity.PeriodDuration)},
                {nameof(TokenDistributionEntity.PeriodIndex)}
              ) VALUES (
                @{nameof(TokenDistributionEntity.TokenId)},
                @{nameof(TokenDistributionEntity.MiningGovernanceId)},
                @{nameof(TokenDistributionEntity.Owner)},
                @{nameof(TokenDistributionEntity.Genesis)},
                @{nameof(TokenDistributionEntity.PeriodDuration)},
                @{nameof(TokenDistributionEntity.PeriodIndex)}
              );";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE token_distribution 
                SET 
                    {nameof(TokenDistributionEntity.Owner)} = @{nameof(TokenDistributionEntity.Owner)},
                    {nameof(TokenDistributionEntity.PeriodIndex)} = @{nameof(TokenDistributionEntity.PeriodIndex)}
                WHERE {nameof(TokenDistributionEntity.Id)} = @{nameof(TokenDistributionEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTokenDistributionCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenDistributionCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistTokenDistributionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<TokenDistributionEntity>(request.TokenDistribution);

                var isUpdate = poolEntity.Id > 1;
                
                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, poolEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? poolEntity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {nameof(TokenDistributionEntity)} record.");
                
                return 0;
            }
        }
    }
}