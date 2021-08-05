using System;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using System.Threading;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class MakeTokenCommandHandler : IRequestHandler<MakeTokenCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeTokenCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeTokenCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistTokenCommand(request.Token));
        }
    }
}
