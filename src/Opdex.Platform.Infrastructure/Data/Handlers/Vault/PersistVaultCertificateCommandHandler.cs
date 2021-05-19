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
    public class PersistVaultCertificateCommandHandler : IRequestHandler<PersistVaultCertificateCommand, bool>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO odx_vault_certificate (
                {nameof(VaultCertificateEntity.VaultId)},
                {nameof(VaultCertificateEntity.Owner)},
                {nameof(VaultCertificateEntity.Amount)},
                {nameof(VaultCertificateEntity.VestedBlock)},
                {nameof(VaultCertificateEntity.Redeemed)},
                {nameof(VaultCertificateEntity.CreatedBlock)},
                {nameof(VaultCertificateEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultCertificateEntity.VaultId)},
                @{nameof(VaultCertificateEntity.Owner)},
                @{nameof(VaultCertificateEntity.Amount)},
                @{nameof(VaultCertificateEntity.VestedBlock)},
                @{nameof(VaultCertificateEntity.Redeemed)},
                @{nameof(VaultCertificateEntity.CreatedBlock)},
                @{nameof(VaultCertificateEntity.ModifiedBlock)}
              );";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE odx_vault_certificate 
                SET 
                    {nameof(VaultCertificateEntity.Amount)} = @{nameof(VaultCertificateEntity.Amount)},
                    {nameof(VaultCertificateEntity.Redeemed)} = @{nameof(VaultCertificateEntity.Redeemed)},
                    {nameof(VaultCertificateEntity.ModifiedBlock)} = @{nameof(VaultCertificateEntity.ModifiedBlock)}
                WHERE {nameof(VaultCertificateEntity.Id)} = @{nameof(VaultCertificateEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistVaultCertificateCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistVaultCertificateCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(PersistVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<VaultCertificateEntity>(request.VaultCertificate);

                var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                return await _context.ExecuteCommandAsync(command) >= 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {nameof(request.VaultCertificate)}.");
                
                return false;
            }
        }
    }
}