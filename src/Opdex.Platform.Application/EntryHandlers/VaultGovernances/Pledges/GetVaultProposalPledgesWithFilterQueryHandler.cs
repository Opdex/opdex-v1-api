using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Pledges;

public class GetVaultProposalPledgesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultProposalPledgesWithFilterQuery, VaultProposalPledgesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> _pledgeAssembler;
    private readonly ILogger<GetVaultProposalPledgesWithFilterQueryHandler> _logger;

    public GetVaultProposalPledgesWithFilterQueryHandler(IMediator mediator, IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto> pledgeAssembler, ILogger<GetVaultProposalPledgesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _pledgeAssembler = pledgeAssembler ?? throw new ArgumentNullException(nameof(pledgeAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<VaultProposalPledgesDto> Handle(GetVaultProposalPledgesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var pledges = await _mediator.Send(new RetrieveVaultProposalPledgesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var pledgesResults = pledges.ToList();

        _logger.LogTrace("Retrieved queried pledges");

        var cursorDto = BuildCursorDto(pledgesResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", pledgesResults.Count);

        var assembledResults = await Task.WhenAll(pledgesResults.Select(pledge => _pledgeAssembler.Assemble(pledge)));

        _logger.LogTrace("Assembled results");

        return new VaultProposalPledgesDto { Pledges = assembledResults, Cursor = cursorDto };
    }
}
