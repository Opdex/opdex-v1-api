using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;

public class SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler
    : IRequestHandler<SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery, VaultProposalPledge>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalPledgeEntity.Id)},
                {nameof(VaultProposalPledgeEntity.VaultGovernanceId)},
                {nameof(VaultProposalPledgeEntity.ProposalId)},
                {nameof(VaultProposalPledgeEntity.Pledger)},
                {nameof(VaultProposalPledgeEntity.Pledge)},
                {nameof(VaultProposalPledgeEntity.Balance)},
                {nameof(VaultProposalPledgeEntity.CreatedBlock)},
                {nameof(VaultProposalPledgeEntity.ModifiedBlock)}
            FROM vault_proposal_pledge
            WHERE {nameof(VaultProposalPledgeEntity.VaultGovernanceId)} = @{nameof(SqlParams.VaultId)}
                AND {nameof(VaultProposalPledgeEntity.ProposalId)} = @{nameof(SqlParams.ProposalId)}
                AND {nameof(VaultProposalPledgeEntity.Pledger)} = @{nameof(SqlParams.Pledger)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultProposalPledge> Handle(SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId, request.ProposalId, request.Pledger), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultProposalPledgeEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(VaultProposalPledge)} not found.");
        }

        return result == null ? null : _mapper.Map<VaultProposalPledge>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong vaultId, ulong proposalId, Address pledger)
        {
            VaultId = vaultId;
            ProposalId = proposalId;
            Pledger = pledger;
        }

        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Pledger { get; }
    }
}
