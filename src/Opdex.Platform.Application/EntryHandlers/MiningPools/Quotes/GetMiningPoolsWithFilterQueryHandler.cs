using MediatR;
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

    public GetMiningPoolsWithFilterQueryHandler(IMediator mediator, IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
    }

    public override async Task<MiningPoolsDto> Handle(GetMiningPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var miningPools = await _mediator.Send(new RetrieveMiningPoolsWithFilterQuery(request.Cursor), cancellationToken);

        var miningPoolsResults = miningPools.ToList();

        var cursorDto = BuildCursorDto(miningPoolsResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = await Task.WhenAll(miningPoolsResults.Select(miningPool => _miningPoolAssembler.Assemble(miningPool)));

        return new MiningPoolsDto { MiningPools = assembledResults, Cursor = cursorDto };
    }
}