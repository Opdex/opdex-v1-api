using AutoMapper;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;

public class PersistVaultProposalVoteCommandHandler : IRequestHandler<PersistVaultProposalVoteCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO vault_proposal_vote (
                {nameof(VaultProposalVoteEntity.VaultGovernanceId)},
                {nameof(VaultProposalVoteEntity.ProposalId)},
                {nameof(VaultProposalVoteEntity.Voter)},
                {nameof(VaultProposalVoteEntity.Vote)},
                {nameof(VaultProposalVoteEntity.Balance)},
                {nameof(VaultProposalVoteEntity.InFavor)},
                {nameof(VaultProposalVoteEntity.CreatedBlock)},
                {nameof(VaultProposalVoteEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(VaultProposalVoteEntity.VaultGovernanceId)},
                @{nameof(VaultProposalVoteEntity.ProposalId)},
                @{nameof(VaultProposalVoteEntity.Voter)},
                @{nameof(VaultProposalVoteEntity.Vote)},
                @{nameof(VaultProposalVoteEntity.Balance)},
                @{nameof(VaultProposalVoteEntity.InFavor)},
                @{nameof(VaultProposalVoteEntity.CreatedBlock)},
                @{nameof(VaultProposalVoteEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE vault_proposal_vote
                SET
                    {nameof(VaultProposalVoteEntity.Vote)} = @{nameof(VaultProposalVoteEntity.Vote)},
                    {nameof(VaultProposalVoteEntity.Balance)} = @{nameof(VaultProposalVoteEntity.Balance)},
                    {nameof(VaultProposalVoteEntity.InFavor)} = @{nameof(VaultProposalVoteEntity.InFavor)},
                    {nameof(VaultProposalVoteEntity.ModifiedBlock)} = @{nameof(VaultProposalVoteEntity.ModifiedBlock)}
                WHERE {nameof(VaultProposalVoteEntity.Id)} = @{nameof(VaultProposalVoteEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public PersistVaultProposalVoteCommandHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ulong> Handle(PersistVaultProposalVoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<VaultProposalVoteEntity>(request.Vote);

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
