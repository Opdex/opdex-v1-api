using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Tokens.Wrapped;

public class MakeTokenWrappedCommandHandler : IRequestHandler<MakeTokenWrappedCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeTokenWrappedCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeTokenWrappedCommand request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new PersistTokenWrappedCommand(request.Wrapped), CancellationToken.None);
    }
}
