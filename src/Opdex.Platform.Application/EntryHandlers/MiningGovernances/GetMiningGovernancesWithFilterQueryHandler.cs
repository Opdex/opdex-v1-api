using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances;

public class GetMiningGovernancesWithFilterQueryHandler : EntryFilterQueryHandler<GetMiningGovernancesWithFilterQuery, MiningGovernancesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<MiningGovernance, MiningGovernanceDto> _miningGovernanceAssembler;
    private readonly ILogger<GetMiningGovernancesWithFilterQueryHandler> _logger;

    public GetMiningGovernancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<MiningGovernance, MiningGovernanceDto> vaultAssembler, ILogger<GetMiningGovernancesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _miningGovernanceAssembler = vaultAssembler ?? throw new ArgumentNullException(nameof(vaultAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<MiningGovernancesDto> Handle(GetMiningGovernancesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var miningGovernances = await _mediator.Send(new RetrieveMiningGovernancesWithFilterQuery(request.Cursor), cancellationToken);

        var miningGovernancesResults = miningGovernances.ToList();

        _logger.LogTrace("Retrieved queried mining governances");

        var cursorDto = BuildCursorDto(miningGovernancesResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", miningGovernancesResults.Count);

        var assembledResults = await Task.WhenAll(miningGovernancesResults.Select(vault => _miningGovernanceAssembler.Assemble(vault)));

        _logger.LogTrace("Assembled results");

        return new MiningGovernancesDto { MiningGovernances = assembledResults, Cursor = cursorDto };
    }
}
