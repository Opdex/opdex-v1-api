using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Proposals;

public class SelectVaultProposalByVaultIdAndPublicIdQueryHandler
    : IRequestHandler<SelectVaultProposalByVaultIdAndPublicIdQuery, VaultProposal>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalEntity.Id)},
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
            FROM vault_proposal
            WHERE {nameof(VaultProposalEntity.VaultId)} = @{nameof(SqlParams.VaultId)}
                AND {nameof(VaultProposalEntity.PublicId)} = @{nameof(SqlParams.PublicId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalByVaultIdAndPublicIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultProposal> Handle(SelectVaultProposalByVaultIdAndPublicIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId, request.PublicId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultProposalEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Proposal not found.");
        }

        return result == null ? null : _mapper.Map<VaultProposal>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong vaultId, ulong publicId)
        {
            VaultId = vaultId;
            PublicId = publicId;
        }

        public ulong VaultId { get; }
        public ulong PublicId { get; }
    }
}
