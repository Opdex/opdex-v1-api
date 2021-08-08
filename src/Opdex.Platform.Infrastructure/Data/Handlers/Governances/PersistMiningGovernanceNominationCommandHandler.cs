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
    public class PersistMiningGovernanceNominationCommandHandler : IRequestHandler<PersistMiningGovernanceNominationCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO governance_nomination (
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                @{nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                @{nameof(MiningGovernanceNominationEntity.IsNominated)},
                @{nameof(MiningGovernanceNominationEntity.Weight)},
                @{nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                @{nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE governance_nomination
                SET
                    {nameof(MiningGovernanceNominationEntity.Weight)} = @{nameof(MiningGovernanceNominationEntity.Weight)},
                    {nameof(MiningGovernanceNominationEntity.IsNominated)} = @{nameof(MiningGovernanceNominationEntity.IsNominated)},
                    {nameof(MiningGovernanceNominationEntity.ModifiedBlock)} = @{nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
                WHERE {nameof(MiningGovernanceNominationEntity.Id)} = @{nameof(MiningGovernanceNominationEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistMiningGovernanceNominationCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMiningGovernanceNominationCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistMiningGovernanceNominationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<MiningGovernanceNominationEntity>(request.Nomination);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, entity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {request.Nomination}.");

                return 0;
            }
        }
    }
}
