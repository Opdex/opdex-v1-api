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

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;

public class PersistVaultProposalCommandHandler : IRequestHandler<PersistVaultProposalCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_proposal (
                {nameof(VaultProposalEntity.PublicId)},
                {nameof(VaultProposalEntity.VaultId)},
                {nameof(VaultProposalEntity.Creator)},
                {nameof(VaultProposalEntity.Wallet)},
                {nameof(VaultProposalEntity.Amount)},
                {nameof(VaultProposalEntity.Description)},
                {nameof(VaultProposalEntity.ProposalTypeId)},
                {nameof(VaultProposalEntity.ProposalStatusId)},
                {nameof(VaultProposalEntity.Expiration)},
                {nameof(VaultProposalEntity.YesAmount)},
                {nameof(VaultProposalEntity.NoAmount)},
                {nameof(VaultProposalEntity.PledgeAmount)},
                {nameof(VaultProposalEntity.Approved)},
                {nameof(VaultProposalEntity.CreatedBlock)},
                {nameof(VaultProposalEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultProposalEntity.PublicId)},
                @{nameof(VaultProposalEntity.VaultId)},
                @{nameof(VaultProposalEntity.Creator)},
                @{nameof(VaultProposalEntity.Wallet)},
                @{nameof(VaultProposalEntity.Amount)},
                @{nameof(VaultProposalEntity.Description)},
                @{nameof(VaultProposalEntity.ProposalTypeId)},
                @{nameof(VaultProposalEntity.ProposalStatusId)},
                @{nameof(VaultProposalEntity.Expiration)},
                @{nameof(VaultProposalEntity.YesAmount)},
                @{nameof(VaultProposalEntity.NoAmount)},
                @{nameof(VaultProposalEntity.PledgeAmount)},
                @{nameof(VaultProposalEntity.Approved)},
                @{nameof(VaultProposalEntity.CreatedBlock)},
                @{nameof(VaultProposalEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_proposal
                SET
                    {nameof(VaultProposalEntity.ProposalStatusId)} = @{nameof(VaultProposalEntity.ProposalStatusId)},
                    {nameof(VaultProposalEntity.Expiration)} = @{nameof(VaultProposalEntity.Expiration)},
                    {nameof(VaultProposalEntity.YesAmount)} = @{nameof(VaultProposalEntity.YesAmount)},
                    {nameof(VaultProposalEntity.NoAmount)} = @{nameof(VaultProposalEntity.NoAmount)},
                    {nameof(VaultProposalEntity.PledgeAmount)} = @{nameof(VaultProposalEntity.PledgeAmount)},
                    {nameof(VaultProposalEntity.Approved)} = @{nameof(VaultProposalEntity.Approved)},
                    {nameof(VaultProposalEntity.ModifiedBlock)} = @{nameof(VaultProposalEntity.ModifiedBlock)}
                WHERE {nameof(VaultProposalEntity.Id)} = @{nameof(VaultProposalEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly ILogger<PersistVaultProposalCommandHandler> _logger;
    private readonly IMapper _mapper;

    public PersistVaultProposalCommandHandler(IDbContext context, ILogger<PersistVaultProposalCommandHandler> logger, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultProposalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultProposalEntity>(request.Proposal);

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
                { "VaultId", request.Proposal.VaultId },
                { "PublicId", request.Proposal.PublicId },
                { "Wallet", request.Proposal.Wallet },
                { "BlockHeight", request.Proposal.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, "Failure persisting vault proposal.");
            }
            return 0;
        }
    }
}
