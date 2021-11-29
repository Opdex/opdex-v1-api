using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances
{
    public class GetMiningGovernanceByAddressQueryHandler : IRequestHandler<GetMiningGovernanceByAddressQuery, MiningGovernanceDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<MiningGovernance, MiningGovernanceDto> _miningGovernanceAssembler;

        public GetMiningGovernanceByAddressQueryHandler(IMediator mediator, IModelAssembler<MiningGovernance, MiningGovernanceDto> miningGovernanceAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _miningGovernanceAssembler = miningGovernanceAssembler ?? throw new ArgumentNullException(nameof(miningGovernanceAssembler));
        }

        public async Task<MiningGovernanceDto> Handle(GetMiningGovernanceByAddressQuery request, CancellationToken cancellationToken)
        {
            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Address), cancellationToken);

            return await _miningGovernanceAssembler.Assemble(miningGovernance);
        }
    }
}
