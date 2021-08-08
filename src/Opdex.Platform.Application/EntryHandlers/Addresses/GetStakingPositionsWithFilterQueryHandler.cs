using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetStakingPositionsWithFilterQueryHandler : EntryFilterQueryHandler<GetStakingPositionsWithFilterQuery, StakingPositionsDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<AddressStaking, StakingPositionDto> _stakingPositionAssembler;

        public GetStakingPositionsWithFilterQueryHandler(IMediator mediator, IModelAssembler<AddressStaking, StakingPositionDto> stakingPositionAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _stakingPositionAssembler = stakingPositionAssembler ?? throw new ArgumentNullException(nameof(stakingPositionAssembler));
        }

        public override async Task<StakingPositionsDto> Handle(GetStakingPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var positions = await _mediator.Send(new RetrieveStakingPositionsWithFilterQuery(request.Address, request.Cursor), cancellationToken);

            var positionsResults = positions.ToList();

            var cursorDto = BuildCursorDto(positionsResults, request.Cursor, pointerSelector: result => result.Id);

            var dtos = await Task.WhenAll(positionsResults.Select(position => _stakingPositionAssembler.Assemble(position)));

            return new StakingPositionsDto { Positions = dtos, Cursor = cursorDto };
        }
    }
}
