using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Certificates;

public class PersistVaultGovernanceCertificateCommandHandler : IRequestHandler<PersistVaultGovernanceCertificateCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_governance_certificate (
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
        $@"UPDATE vault_governance_certificate
                SET
                    {nameof(VaultCertificateEntity.Amount)} = @{nameof(VaultCertificateEntity.Amount)},
                    {nameof(VaultCertificateEntity.Revoked)} = @{nameof(VaultCertificateEntity.Revoked)},
                    {nameof(VaultCertificateEntity.Redeemed)} = @{nameof(VaultCertificateEntity.Redeemed)},
                    {nameof(VaultCertificateEntity.ModifiedBlock)} = @{nameof(VaultCertificateEntity.ModifiedBlock)}
                WHERE {nameof(VaultCertificateEntity.Id)} = @{nameof(VaultCertificateEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public PersistVaultGovernanceCertificateCommandHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultGovernanceCertificateCommand request, CancellationToken cancellationToken)
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
        catch (Exception)
        {
            // TODO: PAPI-276
            return 0;
        }
    }
}
