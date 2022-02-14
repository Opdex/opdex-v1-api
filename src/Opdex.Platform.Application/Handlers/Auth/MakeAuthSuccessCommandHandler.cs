using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Auth;

public class MakeAuthSuccessCommandHandler : IRequestHandler<MakeAuthSuccessCommand, bool>
{
    private readonly IMediator _mediator;

    public MakeAuthSuccessCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MakeAuthSuccessCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistAuthSuccessCommand(request.AuthSuccess), CancellationToken.None);
    }
}
