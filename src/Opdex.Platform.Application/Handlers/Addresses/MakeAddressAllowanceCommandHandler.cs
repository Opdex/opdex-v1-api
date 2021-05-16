using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class MakeAddressAllowanceCommandHandler : IRequestHandler<MakeAddressAllowanceCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeAddressAllowanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeAddressAllowanceCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistAddressAllowanceCommand(request.AddressAllowance), cancellationToken);
        }
    }
}