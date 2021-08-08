using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class MakeAddressMiningCommandHandler : IRequestHandler<MakeAddressMiningCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeAddressMiningCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeAddressMiningCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistAddressMiningCommand(request.AddressMining), cancellationToken);
        }
    }
}