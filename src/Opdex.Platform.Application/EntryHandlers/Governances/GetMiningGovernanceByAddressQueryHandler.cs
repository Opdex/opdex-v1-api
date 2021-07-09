using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class GetMiningGovernanceByAddressQueryHandler : IRequestHandler<GetMiningGovernanceByAddressQuery, MiningGovernanceDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<MiningGovernance, MiningGovernanceDto> _governanceAssembler;

        public GetMiningGovernanceByAddressQueryHandler(IMediator mediator, IModelAssembler<MiningGovernance, MiningGovernanceDto> governanceAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _governanceAssembler = governanceAssembler ?? throw new ArgumentNullException(nameof(governanceAssembler));
        }

        public async Task<MiningGovernanceDto> Handle(GetMiningGovernanceByAddressQuery request, CancellationToken cancellationToken)
        {
            var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Address), cancellationToken);

            return await _governanceAssembler.Assemble(miningGovernance);
        }
    }
}
