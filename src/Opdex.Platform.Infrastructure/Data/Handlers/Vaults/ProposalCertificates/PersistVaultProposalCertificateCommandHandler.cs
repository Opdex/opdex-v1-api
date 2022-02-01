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

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.ProposalCertificates;

public class PersistVaultProposalCertificateCommandHandler : IRequestHandler<PersistVaultProposalCertificateCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_proposal_certificate (
                {nameof(VaultProposalCertificateEntity.ProposalId)},
                {nameof(VaultProposalCertificateEntity.CertificateId)},
                {nameof(VaultProposalCertificateEntity.CreatedBlock)},
                {nameof(VaultProposalCertificateEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultProposalCertificateEntity.ProposalId)},
                @{nameof(VaultProposalCertificateEntity.CertificateId)},
                @{nameof(VaultProposalCertificateEntity.CreatedBlock)},
                @{nameof(VaultProposalCertificateEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly ILogger<PersistVaultProposalCertificateCommandHandler> _logger;
    private readonly IMapper _mapper;

    public PersistVaultProposalCertificateCommandHandler(IDbContext context, ILogger<PersistVaultProposalCertificateCommandHandler> logger, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ulong> Handle(PersistVaultProposalCertificateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ProposalCertificate.Id > 0) return request.ProposalCertificate.Id;

            var entity = _mapper.Map<VaultProposalCertificateEntity>(request.ProposalCertificate);

            var command = DatabaseQuery.Create(InsertSqlCommand, entity, cancellationToken);

            return await _context.ExecuteScalarAsync<ulong>(command);
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "ProposalId", request.ProposalCertificate.ProposalId },
                { "CertificateId", request.ProposalCertificate.CertificateId },
                { "BlockHeight", request.ProposalCertificate.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, "Failure persisting vault proposal certificate.");
            }

            return 0;
        }
    }
}
