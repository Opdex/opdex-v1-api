using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens;

public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, ulong>
{
    private readonly IMediator _mediator;

    public CreateTokenCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: false), CancellationToken.None);
        var tokenId = token?.Id ?? 0ul;

        if (token == null)
        {
            var summary = await _mediator.Send(new CallCirrusGetStandardTokenContractSummaryQuery(request.Token,
                                                                                                  request.BlockHeight,
                                                                                                  includeBaseProperties: true,
                                                                                                  includeTotalSupply: true));

            token = new Token(request.Token,
                              summary.Name,
                              summary.Symbol,
                              (int)summary.Decimals.GetValueOrDefault(),
                              summary.Sats.GetValueOrDefault(),
                              summary.TotalSupply.GetValueOrDefault(),
                              request.BlockHeight);

            tokenId = await _mediator.Send(new MakeTokenCommand(token, request.BlockHeight));
        }

        await Task.WhenAll(request.Attributes.Select(attribute =>
        {
            var tokenAttribute = new TokenAttribute(tokenId, attribute);
            return _mediator.Send(new MakeTokenAttributeCommand(tokenAttribute), CancellationToken.None);
        }));

        return tokenId;
    }
}
