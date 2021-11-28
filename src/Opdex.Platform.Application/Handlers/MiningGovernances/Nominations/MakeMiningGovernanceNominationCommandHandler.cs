using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances.Nominations
{
    public class MakeMiningGovernanceNominationCommandHandler : IRequestHandler<MakeMiningGovernanceNominationCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeMiningGovernanceNominationCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<ulong> Handle(MakeMiningGovernanceNominationCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMiningGovernanceNominationCommand(request.Nomination), cancellationToken);
        }
    }
}
