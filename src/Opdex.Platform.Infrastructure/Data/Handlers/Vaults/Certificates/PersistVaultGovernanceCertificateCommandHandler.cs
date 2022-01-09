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

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;

public class PersistVaultCertificateCommandHandler : IRequestHandler<PersistVaultCertificateCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_certificate (
                {nameof(VaultCertificateEntity.VaultId)},
                {nameof(VaultCertificateEntity.Owner)},
                {nameof(VaultCertificateEntity.Amount)},
                {nameof(VaultCertificateEntity.Revoked)},
                {nameof(VaultCertificateEntity.Redeemed)},
                {nameof(VaultCertificateEntity.VestedBlock)},
                {nameof(VaultCertificateEntity.CreatedBlock)},
                {nameof(VaultCertificateEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultCertificateEntity.VaultId)},
                @{nameof(VaultCertificateEntity.Owner)},
                @{nameof(VaultCertificateEntity.Amount)},
                @{nameof(VaultCertificateEntity.Revoked)},
                @{nameof(VaultCertificateEntity.Redeemed)},
                @{nameof(VaultCertificateEntity.VestedBlock)},
                @{nameof(VaultCertificateEntity.CreatedBlock)},
                @{nameof(VaultCertificateEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_certificate
                SET
                    {nameof(VaultCertificateEntity.Amount)} = @{nameof(VaultCertificateEntity.Amount)},
                    {nameof(VaultCertificateEntity.Revoked)} = @{nameof(VaultCertificateEntity.Revoked)},
                    {nameof(VaultCertificateEntity.Redeemed)} = @{nameof(VaultCertificateEntity.Redeemed)},
                    {nameof(VaultCertificateEntity.ModifiedBlock)} = @{nameof(VaultCertificateEntity.ModifiedBlock)}
                WHERE {nameof(VaultCertificateEntity.Id)} = @{nameof(VaultCertificateEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly ILogger<PersistVaultCertificateCommandHandler> _logger;
    private readonly IMapper _mapper;

    public PersistVaultCertificateCommandHandler(IDbContext context, ILogger<PersistVaultCertificateCommandHandler> logger, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultCertificateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultCertificateEntity>(request.Certificate);

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
                { "VaultId", request.Certificate.VaultId },
                { "Owner", request.Certificate.Owner },
                { "BlockHeight", request.Certificate.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, "Failure persisting vault certificate.");
            }
            return 0;
        }
    }
}
