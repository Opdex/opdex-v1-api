using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Pledges;

public class GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandler : IRequestHandler<GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery, VaultProposalPledgeDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> _pledgeAssembler;

    public GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandler(IMediator mediator, IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> pledgeAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _pledgeAssembler = pledgeAssembler ?? throw new ArgumentNullException(nameof(pledgeAssembler));
    }
    public async Task<VaultProposalPledgeDto> Handle(GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.PublicProposalId, findOrThrow: true), cancellationToken);
        var pledge = await _mediator.Send(new RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(vault.Id, proposal.Id, request.Pledger, findOrThrow: true), cancellationToken);
        return await _pledgeAssembler.Assemble(pledge);
    }
}
