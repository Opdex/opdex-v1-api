using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

namespace Opdex.Platform.Application.Handlers.Markets;

public class MakeMarketRouterCommandHandler : IRequestHandler<MakeMarketRouterCommand, bool>
{
    private readonly IMediator _mediator;
        
    public MakeMarketRouterCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
        
    public Task<bool> Handle(MakeMarketRouterCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistMarketRouterCommand(request.Router), CancellationToken.None);
    }
}