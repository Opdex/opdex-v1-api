using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.MiningPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;

public class GetMiningPoolsWithFilterQueryHandler : EntryFilterQueryHandler<GetMiningPoolsWithFilterQuery, MiningPoolsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<MiningPool, MiningPoolDto> _miningPoolAssembler;
    private readonly ILogger<GetMiningPoolsWithFilterQueryHandler> _logger;

    public GetMiningPoolsWithFilterQueryHandler(IMediator mediator, IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler, ILogger<GetMiningPoolsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<MiningPoolsDto> Handle(GetMiningPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var miningPools = await _mediator.Send(new RetrieveMiningPoolsWithFilterQuery(request.Cursor), cancellationToken);

        var miningPoolsResults = miningPools.ToList();

        _logger.LogTrace("Retrieved queried mining pools");

        var cursorDto = BuildCursorDto(miningPoolsResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", miningPoolsResults.Count);

        var assembledResults = await Task.WhenAll(miningPoolsResults.Select(miningPool => _miningPoolAssembler.Assemble(miningPool)));

        _logger.LogTrace("Assembled results");

        return new MiningPoolsDto { MiningPools = assembledResults, Cursor = cursorDto };
    }
}
