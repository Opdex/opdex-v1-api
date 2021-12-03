using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;

public class PersistVaultProposalPledgeCommandHandler : IRequestHandler<PersistVaultProposalPledgeCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_proposal_pledge (
                {nameof(VaultProposalPledgeEntity.VaultGovernanceId)},
                {nameof(VaultProposalPledgeEntity.ProposalId)},
                {nameof(VaultProposalPledgeEntity.Pledger)},
                {nameof(VaultProposalPledgeEntity.Pledge)},
                {nameof(VaultProposalPledgeEntity.Balance)},
                {nameof(VaultProposalPledgeEntity.CreatedBlock)},
                {nameof(VaultProposalPledgeEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultProposalPledgeEntity.VaultGovernanceId)},
                @{nameof(VaultProposalPledgeEntity.ProposalId)},
                @{nameof(VaultProposalPledgeEntity.Pledger)},
                @{nameof(VaultProposalPledgeEntity.Pledge)},
                @{nameof(VaultProposalPledgeEntity.Balance)},
                @{nameof(VaultProposalPledgeEntity.CreatedBlock)},
                @{nameof(VaultProposalPledgeEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_proposal_pledge
                SET
                    {nameof(VaultProposalPledgeEntity.Pledge)} = @{nameof(VaultProposalPledgeEntity.Pledge)},
                    {nameof(VaultProposalPledgeEntity.Balance)} = @{nameof(VaultProposalPledgeEntity.Balance)},
                    {nameof(VaultProposalPledgeEntity.ModifiedBlock)} = @{nameof(VaultProposalPledgeEntity.ModifiedBlock)}
                WHERE {nameof(VaultProposalPledgeEntity.Id)} = @{nameof(VaultProposalPledgeEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public PersistVaultProposalPledgeCommandHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultProposalPledgeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultProposalPledgeEntity>(request.Pledge);

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
