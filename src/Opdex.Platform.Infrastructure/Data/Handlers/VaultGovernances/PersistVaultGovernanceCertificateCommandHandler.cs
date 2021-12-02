using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;

public class PersistVaultGovernanceCertificateCommandHandler : IRequestHandler<PersistVaultGovernanceCertificateCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_governance_certificate (
                {nameof(VaultGovernanceCertificateEntity.VaultGovernanceId)},
                {nameof(VaultGovernanceCertificateEntity.Owner)},
                {nameof(VaultGovernanceCertificateEntity.Amount)},
                {nameof(VaultGovernanceCertificateEntity.Revoked)},
                {nameof(VaultGovernanceCertificateEntity.Redeemed)},
                {nameof(VaultGovernanceCertificateEntity.VestedBlock)},
                {nameof(VaultGovernanceCertificateEntity.CreatedBlock)},
                {nameof(VaultGovernanceCertificateEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultGovernanceCertificateEntity.VaultGovernanceId)},
                @{nameof(VaultGovernanceCertificateEntity.Owner)},
                @{nameof(VaultGovernanceCertificateEntity.Amount)},
                @{nameof(VaultGovernanceCertificateEntity.Revoked)},
                @{nameof(VaultGovernanceCertificateEntity.Redeemed)},
                @{nameof(VaultGovernanceCertificateEntity.VestedBlock)},
                @{nameof(VaultGovernanceCertificateEntity.CreatedBlock)},
                @{nameof(VaultGovernanceCertificateEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_governance_certificate
                SET
                    {nameof(VaultGovernanceCertificateEntity.Amount)} = @{nameof(VaultGovernanceCertificateEntity.Amount)},
                    {nameof(VaultGovernanceCertificateEntity.Revoked)} = @{nameof(VaultGovernanceCertificateEntity.Revoked)},
                    {nameof(VaultGovernanceCertificateEntity.Redeemed)} = @{nameof(VaultGovernanceCertificateEntity.Redeemed)},
                    {nameof(VaultGovernanceCertificateEntity.ModifiedBlock)} = @{nameof(VaultGovernanceCertificateEntity.ModifiedBlock)}
                WHERE {nameof(VaultGovernanceCertificateEntity.Id)} = @{nameof(VaultGovernanceCertificateEntity.Id)};";

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
            var entity = _mapper.Map<VaultGovernanceCertificateEntity>(request.Certificate);

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
