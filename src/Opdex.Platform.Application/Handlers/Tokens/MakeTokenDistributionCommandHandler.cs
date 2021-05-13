using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class MakeTokenDistributionCommandHandler : IRequestHandler<MakeTokenDistributionCommand, bool>
    {
        private readonly IMediator _mediator;
        
        public MakeTokenDistributionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<bool> Handle(MakeTokenDistributionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistTokenDistributionCommand(request.TokenDistribution), cancellationToken);
        }
    }
}