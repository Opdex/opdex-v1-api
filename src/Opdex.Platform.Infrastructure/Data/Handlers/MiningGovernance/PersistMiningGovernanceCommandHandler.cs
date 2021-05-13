using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance
{
    public class PersistMiningGovernanceCommandHandler : IRequestHandler<PersistMiningGovernanceCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO odx_mining_governance (
                {nameof(MiningGovernanceEntity.Address)},
                {nameof(MiningGovernanceEntity.TokenId)},
                {nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                {nameof(MiningGovernanceEntity.Balance)},
                {nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                {nameof(MiningGovernanceEntity.MiningPoolReward)}
              ) VALUES (
                @{nameof(MiningGovernanceEntity.Address)},
                @{nameof(MiningGovernanceEntity.TokenId)},
                @{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                @{nameof(MiningGovernanceEntity.Balance)},
                @{nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                @{nameof(MiningGovernanceEntity.MiningPoolReward)}
              );";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE odx_mining_governance 
                SET 
                    {nameof(MiningGovernanceEntity.NominationPeriodEnd)} = @{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                    {nameof(MiningGovernanceEntity.Balance)} = @{nameof(MiningGovernanceEntity.Balance)},
                    {nameof(MiningGovernanceEntity.MiningPoolsFunded)} = @{nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                    {nameof(MiningGovernanceEntity.MiningPoolReward)} = @{nameof(MiningGovernanceEntity.MiningPoolReward)}
                WHERE {nameof(MiningGovernanceEntity.Id)} = @{nameof(MiningGovernanceEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistMiningGovernanceCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMiningGovernanceCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<MiningGovernanceEntity>(request.MiningGovernance);

                var isUpdate = poolEntity.Id > 1;
                
                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, poolEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? poolEntity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {nameof(MiningGovernanceEntity)} record.");
                
                return 0;
            }
        }
    }
}