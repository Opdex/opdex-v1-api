using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances.Nominations
{
    public class MakeMiningGovernanceNominationCommandHandler : IRequestHandler<MakeMiningGovernanceNominationCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeMiningGovernanceNominationCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeMiningGovernanceNominationCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMiningGovernanceNominationCommand(request.Nomination), cancellationToken);
        }
    }
}
