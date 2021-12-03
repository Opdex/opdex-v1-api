using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Mining;

public class MakeAddressMiningCommandHandler : IRequestHandler<MakeAddressMiningCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeAddressMiningCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeAddressMiningCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistAddressMiningCommand(request.AddressMining), cancellationToken);
    }
}