using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;

public class SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler
    : IRequestHandler<SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery, VaultProposalVote>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalVoteEntity.Id)},
                {nameof(VaultProposalVoteEntity.VaultGovernanceId)},
                {nameof(VaultProposalVoteEntity.ProposalId)},
                {nameof(VaultProposalVoteEntity.Voter)},
                {nameof(VaultProposalVoteEntity.Vote)},
                {nameof(VaultProposalVoteEntity.Balance)},
                {nameof(VaultProposalVoteEntity.InFavor)},
                {nameof(VaultProposalVoteEntity.CreatedBlock)},
                {nameof(VaultProposalVoteEntity.ModifiedBlock)}
            FROM vault_proposal_vote
            WHERE {nameof(VaultProposalVoteEntity.VaultGovernanceId)} = @{nameof(SqlParams.VaultId)}
                AND {nameof(VaultProposalVoteEntity.ProposalId)} = @{nameof(SqlParams.ProposalId)}
                AND {nameof(VaultProposalVoteEntity.Voter)} = @{nameof(SqlParams.Voter)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultProposalVote> Handle(SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId, request.ProposalId, request.Voter), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultProposalVoteEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(VaultProposalVote)} not found.");
        }

        return result == null ? null : _mapper.Map<VaultProposalVote>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong vaultId, ulong proposalId, Address voter)
        {
            VaultId = vaultId;
            ProposalId = proposalId;
            Voter = voter;
        }

        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Voter { get; }
    }
}
