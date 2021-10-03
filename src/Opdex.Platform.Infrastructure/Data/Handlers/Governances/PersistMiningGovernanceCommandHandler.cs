using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Governances
{
    public class PersistMiningGovernanceCommandHandler : IRequestHandler<PersistMiningGovernanceCommand, ulong>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO governance (
                {nameof(MiningGovernanceEntity.Address)},
                {nameof(MiningGovernanceEntity.TokenId)},
                {nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                {nameof(MiningGovernanceEntity.MiningDuration)},
                {nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                {nameof(MiningGovernanceEntity.MiningPoolReward)},
                {nameof(MiningGovernanceEntity.CreatedBlock)},
                {nameof(MiningGovernanceEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MiningGovernanceEntity.Address)},
                @{nameof(MiningGovernanceEntity.TokenId)},
                @{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                @{nameof(MiningGovernanceEntity.MiningDuration)},
                @{nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                @{nameof(MiningGovernanceEntity.MiningPoolReward)},
                @{nameof(MiningGovernanceEntity.CreatedBlock)},
                @{nameof(MiningGovernanceEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE governance
                SET
                    {nameof(MiningGovernanceEntity.NominationPeriodEnd)} = @{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                    {nameof(MiningGovernanceEntity.MiningPoolsFunded)} = @{nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                    {nameof(MiningGovernanceEntity.MiningPoolReward)} = @{nameof(MiningGovernanceEntity.MiningPoolReward)},
                    {nameof(MiningGovernanceEntity.ModifiedBlock)} = @{nameof(MiningGovernanceEntity.ModifiedBlock)}
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

        public async Task<ulong> Handle(PersistMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<MiningGovernanceEntity>(request.MiningGovernance);

                var isUpdate = poolEntity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, poolEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<ulong>(command);

                return isUpdate ? poolEntity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {request.MiningGovernance}.");

                return 0;
            }
        }
    }
}
