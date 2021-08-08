using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class MakeAddressStakingCommandHandler : IRequestHandler<MakeAddressStakingCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeAddressStakingCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeAddressStakingCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistAddressStakingCommand(request.AddressStaking), cancellationToken);
        }
    }
}