using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;

public class PersistVaultProposalCommandHandler : IRequestHandler<PersistVaultProposalCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_proposal (
                {nameof(VaultProposalEntity.PublicId)},
                {nameof(VaultProposalEntity.VaultGovernanceId)},
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
                {nameof(VaultProposalEntity.CreatedBlock)},
                {nameof(VaultProposalEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultProposalEntity.PublicId)},
                @{nameof(VaultProposalEntity.VaultGovernanceId)},
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
                    {nameof(VaultProposalEntity.ModifiedBlock)} = @{nameof(VaultProposalEntity.ModifiedBlock)}
                WHERE {nameof(VaultProposalEntity.Id)} = @{nameof(VaultProposalEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public PersistVaultProposalCommandHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
        catch (Exception)
        {
            // TODO: PAPI-276
            return 0;
        }
    }
}