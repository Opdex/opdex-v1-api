using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Summaries;

public class MakeMarketSummaryCommandHandler : IRequestHandler<MakeMarketSummaryCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeMarketSummaryCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeMarketSummaryCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistMarketSummaryCommand(request.Summary), cancellationToken);
    }
}
