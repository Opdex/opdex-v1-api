using System;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using System.Threading;

namespace Opdex.Platform.Application.Handlers.Tokens;

public class MakeTokenCommandHandler : IRequestHandler<MakeTokenCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeTokenCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.Refresh)
        {
            var summary = await _mediator.Send(new CallCirrusGetStandardTokenContractSummaryQuery(request.Token.Address,
                                                                                                  request.BlockHeight,
                                                                                                  includeTotalSupply: request.RefreshTotalSupply), CancellationToken.None);

            request.Token.Update(summary);
        }

        return await _mediator.Send(new PersistTokenCommand(request.Token), CancellationToken.None);
    }
}
