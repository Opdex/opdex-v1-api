using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults;

public class PersistVaultCommandHandler : IRequestHandler<PersistVaultCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault (
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.UnassignedSupply)},
                {nameof(VaultEntity.VestingDuration)},
                {nameof(VaultEntity.ProposedSupply)},
                {nameof(VaultEntity.TotalPledgeMinimum)},
                {nameof(VaultEntity.TotalVoteMinimum)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultEntity.TokenId)},
                @{nameof(VaultEntity.Address)},
                @{nameof(VaultEntity.UnassignedSupply)},
                @{nameof(VaultEntity.VestingDuration)},
                @{nameof(VaultEntity.ProposedSupply)},
                @{nameof(VaultEntity.TotalPledgeMinimum)},
                @{nameof(VaultEntity.TotalVoteMinimum)},
                @{nameof(VaultEntity.CreatedBlock)},
                @{nameof(VaultEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault
                SET
                    {nameof(VaultEntity.UnassignedSupply)} = @{nameof(VaultEntity.UnassignedSupply)},
                    {nameof(VaultEntity.ProposedSupply)} = @{nameof(VaultEntity.ProposedSupply)},
                    {nameof(VaultEntity.TotalPledgeMinimum)} = @{nameof(VaultEntity.TotalPledgeMinimum)},
                    {nameof(VaultEntity.TotalVoteMinimum)} = @{nameof(VaultEntity.TotalVoteMinimum)},
                    {nameof(VaultEntity.ModifiedBlock)} = @{nameof(VaultEntity.ModifiedBlock)}
                WHERE {nameof(VaultEntity.Id)} = @{nameof(VaultEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly ILogger<PersistVaultCommandHandler> _logger;
    private readonly IMapper _mapper;

    public PersistVaultCommandHandler(IDbContext context, ILogger<PersistVaultCommandHandler> logger, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultEntity>(request.Vault);

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
