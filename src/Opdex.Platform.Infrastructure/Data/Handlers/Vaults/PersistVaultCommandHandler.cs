using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class PersistVaultCommandHandler : IRequestHandler<PersistVaultCommand, ulong>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO vault (
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.PendingOwner)},
                {nameof(VaultEntity.Owner)},
                {nameof(VaultEntity.Genesis)},
                {nameof(VaultEntity.UnassignedSupply)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultEntity.Address)},
                @{nameof(VaultEntity.TokenId)},
                @{nameof(VaultEntity.PendingOwner)},
                @{nameof(VaultEntity.Owner)},
                @{nameof(VaultEntity.Genesis)},
                @{nameof(VaultEntity.UnassignedSupply)},
                @{nameof(VaultEntity.CreatedBlock)},
                @{nameof(VaultEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE vault
                SET
                    {nameof(VaultEntity.PendingOwner)} = @{nameof(VaultEntity.PendingOwner)},
                    {nameof(VaultEntity.Owner)} = @{nameof(VaultEntity.Owner)},
                    {nameof(VaultEntity.UnassignedSupply)} = @{nameof(VaultEntity.UnassignedSupply)},
                    {nameof(VaultEntity.ModifiedBlock)} = @{nameof(VaultEntity.ModifiedBlock)}
                WHERE {nameof(VaultEntity.Id)} = @{nameof(VaultEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistVaultCommandHandler(IDbContext context, IMapper mapper,
            ILogger<PersistVaultCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogError(ex, $"Failure persisting {nameof(request.Vault)}.");

                return 0;
            }
        }
    }
}
