using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Mining
{
    public class GetMiningPositionsWithFilterQueryHandler : EntryFilterQueryHandler<GetMiningPositionsWithFilterQuery, MiningPositionsDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<AddressMining, MiningPositionDto> _miningPositionAssembler;

        public GetMiningPositionsWithFilterQueryHandler(IMediator mediator, IModelAssembler<AddressMining, MiningPositionDto> miningPositionAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _miningPositionAssembler = miningPositionAssembler ?? throw new ArgumentNullException(nameof(miningPositionAssembler));
        }

        public override async Task<MiningPositionsDto> Handle(GetMiningPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var positions = await _mediator.Send(new RetrieveMiningPositionsWithFilterQuery(request.Address, request.Cursor), cancellationToken);

            var positionsResults = positions.ToList();

            var cursorDto = BuildCursorDto(positionsResults, request.Cursor, pointerSelector: result => result.Id);

            var dtos = await Task.WhenAll(positionsResults.Select(position => _miningPositionAssembler.Assemble(position)));

            return new MiningPositionsDto { Positions = dtos, Cursor = cursorDto };
        }
    }
}
