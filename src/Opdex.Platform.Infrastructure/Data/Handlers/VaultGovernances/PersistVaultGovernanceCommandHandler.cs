using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;

public class PersistVaultGovernanceCommandHandler : IRequestHandler<PersistVaultGovernanceCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_governance (
                {nameof(VaultGovernanceEntity.TokenId)},
                {nameof(VaultGovernanceEntity.Address)},
                {nameof(VaultGovernanceEntity.UnassignedSupply)},
                {nameof(VaultGovernanceEntity.VestingDuration)},
                {nameof(VaultGovernanceEntity.ProposedSupply)},
                {nameof(VaultGovernanceEntity.TotalPledgeMinimum)},
                {nameof(VaultGovernanceEntity.TotalVoteMinimum)},
                {nameof(VaultGovernanceEntity.CreatedBlock)},
                {nameof(VaultGovernanceEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultGovernanceEntity.TokenId)},
                @{nameof(VaultGovernanceEntity.Address)},
                @{nameof(VaultGovernanceEntity.UnassignedSupply)},
                @{nameof(VaultGovernanceEntity.VestingDuration)},
                @{nameof(VaultGovernanceEntity.ProposedSupply)},
                @{nameof(VaultGovernanceEntity.TotalPledgeMinimum)},
                @{nameof(VaultGovernanceEntity.TotalVoteMinimum)},
                @{nameof(VaultGovernanceEntity.CreatedBlock)},
                @{nameof(VaultGovernanceEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_governance
                SET
                    {nameof(VaultGovernanceEntity.UnassignedSupply)} = @{nameof(VaultGovernanceEntity.UnassignedSupply)},
                    {nameof(VaultGovernanceEntity.ProposedSupply)} = @{nameof(VaultGovernanceEntity.ProposedSupply)},
                    {nameof(VaultGovernanceEntity.TotalPledgeMinimum)} = @{nameof(VaultGovernanceEntity.TotalPledgeMinimum)},
                    {nameof(VaultGovernanceEntity.TotalVoteMinimum)} = @{nameof(VaultGovernanceEntity.TotalVoteMinimum)},
                    {nameof(VaultGovernanceEntity.ModifiedBlock)} = @{nameof(VaultGovernanceEntity.ModifiedBlock)}
                WHERE {nameof(VaultGovernanceEntity.Id)} = @{nameof(VaultGovernanceEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly ILogger<PersistVaultGovernanceCommandHandler> _logger;
    private readonly IMapper _mapper;

    public PersistVaultGovernanceCommandHandler(IDbContext context, ILogger<PersistVaultGovernanceCommandHandler> logger, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultGovernanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultGovernanceEntity>(request.Vault);

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
                { "Contract", request.Vault.Address },
                { "BlockHeight", request.Vault.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, "Failure persisting vault.");
            }
            return 0;
        }
    }
}
