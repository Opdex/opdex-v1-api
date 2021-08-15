using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class GetMiningGovernancesWithFilterQueryHandler : EntryFilterQueryHandler<GetMiningGovernancesWithFilterQuery, MiningGovernancesDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<MiningGovernance, MiningGovernanceDto> _miningGovernanceAssembler;

        public GetMiningGovernancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<MiningGovernance, MiningGovernanceDto> vaultAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _miningGovernanceAssembler = vaultAssembler ?? throw new ArgumentNullException(nameof(vaultAssembler));
        }

        public override async Task<MiningGovernancesDto> Handle(GetMiningGovernancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var miningGovernances = await _mediator.Send(new RetrieveMiningGovernancesWithFilterQuery(request.Cursor), cancellationToken);

            var miningGovernancesResults = miningGovernances.ToList();

            var cursorDto = BuildCursorDto(miningGovernancesResults, request.Cursor, pointerSelector: result => result.Id);

            var assembledResults = await Task.WhenAll(miningGovernancesResults.Select(vault => _miningGovernanceAssembler.Assemble(vault)));

            return new MiningGovernancesDto { Governances = assembledResults, Cursor = cursorDto };
        }
    }
}
