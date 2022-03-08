using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Enums;
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
    private readonly ILogger<CreateTokenCommandHandler> _logger;

    public CreateTokenCommandHandler(IMediator mediator, ILogger<CreateTokenCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var attributes = request.Attributes;
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: false), CancellationToken.None);
        var tokenId = token?.Id ?? 0ul;

        if (token is null)
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

            tokenId = await _mediator.Send(new MakeTokenCommand(token, request.BlockHeight), CancellationToken.None);
        }

        if (tokenId == 0)
        {
            _logger.LogError("Something went wrong indexing the token");
            return tokenId;
        }

        var interfluxSummary = await _mediator.Send(new CallCirrusGetInterfluxTokenContractSummaryQuery(request.Token, request.BlockHeight), CancellationToken.None);
        if (interfluxSummary is not null)
        {
            attributes = request.Attributes.Append(TokenAttributeType.Interflux);

            var tokenWrapped = new TokenWrapped(tokenId, interfluxSummary.Owner, interfluxSummary.NativeChain, interfluxSummary.NativeAddress, request.BlockHeight);
            var tokenWrappedId = await _mediator.Send(new MakeTokenWrappedCommand(tokenWrapped), CancellationToken.None);
            if (tokenWrappedId == 0) _logger.LogError("Something went wrong indexing the wrapped token mapping");
        }

        var attributesPersisted = await Task.WhenAll(attributes.Select(attribute =>
        {
            var tokenAttribute = new TokenAttribute(tokenId, attribute);
            return _mediator.Send(new MakeTokenAttributeCommand(tokenAttribute), CancellationToken.None);
        }));

        if (attributesPersisted.Any(a => !a)) _logger.LogError("Something went wrong indexing the token attributes");

        return tokenId;
    }
}
