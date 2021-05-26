using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;

namespace Opdex.Platform.Application.Handlers.Deployers
{
    public class MakeDeployerCommandHandler : IRequestHandler<MakeDeployerCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeDeployerCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<long> Handle(MakeDeployerCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistDeployerCommand(request.Deployer), cancellationToken);
        }
    }
}