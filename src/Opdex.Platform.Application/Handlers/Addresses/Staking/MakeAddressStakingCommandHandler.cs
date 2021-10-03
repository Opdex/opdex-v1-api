using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Staking
{
    public class MakeAddressStakingCommandHandler : IRequestHandler<MakeAddressStakingCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeAddressStakingCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<ulong> Handle(MakeAddressStakingCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistAddressStakingCommand(request.AddressStaking), cancellationToken);
        }
    }
}
