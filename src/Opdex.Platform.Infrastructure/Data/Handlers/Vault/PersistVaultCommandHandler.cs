using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vault
{
    public class PersistVaultCommandHandler : IRequestHandler<PersistVaultCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO odx_vault (
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.Owner)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultEntity.Address)},
                @{nameof(VaultEntity.TokenId)},
                @{nameof(VaultEntity.Owner)},
                @{nameof(VaultEntity.CreatedBlock)},
                @{nameof(VaultEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE address_allowance 
                SET 
                    {nameof(VaultEntity.Owner)} = @{nameof(VaultEntity.Owner)},
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

        public async Task<long> Handle(PersistVaultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<VaultEntity>(request.Vault);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                var result = await _context.ExecuteScalarAsync<long>(command);
                
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