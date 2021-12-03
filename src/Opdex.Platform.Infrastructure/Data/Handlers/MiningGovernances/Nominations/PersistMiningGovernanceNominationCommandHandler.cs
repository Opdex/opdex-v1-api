using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;

public class PersistMiningGovernanceNominationCommandHandler : IRequestHandler<PersistMiningGovernanceNominationCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO mining_governance_nomination (
                {nameof(MiningGovernanceNominationEntity.MiningGovernanceId)},
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MiningGovernanceNominationEntity.MiningGovernanceId)},
                @{nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                @{nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                @{nameof(MiningGovernanceNominationEntity.IsNominated)},
                @{nameof(MiningGovernanceNominationEntity.Weight)},
                @{nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                @{nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();";

    private static readonly string UpdateSqlCommand =
        $@"UPDATE mining_governance_nomination
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

    public async Task<ulong> Handle(PersistMiningGovernanceNominationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MiningGovernanceNominationEntity>(request.Nomination);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure persisting {request.Nomination}.");

            return 0;
        }
    }
}