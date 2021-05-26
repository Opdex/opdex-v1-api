using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance;

namespace Opdex.Platform.Application.Handlers.MiningGovernance
{
    public class MakeMiningGovernanceCommandHandler : IRequestHandler<MakeMiningGovernanceCommand, long>
    {
        private readonly IMediator _mediator;
        
        public MakeMiningGovernanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<long> Handle(MakeMiningGovernanceCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMiningGovernanceCommand(request.MiningGovernance), cancellationToken);
        }
    }
}