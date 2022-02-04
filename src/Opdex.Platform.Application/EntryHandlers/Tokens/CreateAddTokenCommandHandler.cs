using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Exceptions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens;

public class CreateAddTokenCommandHandler : IRequestHandler<CreateAddTokenCommand, TokenDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateAddTokenCommandHandler> _logger;

    public CreateAddTokenCommandHandler(IMapper mapper, IMediator mediator, ILogger<CreateAddTokenCommandHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TokenDto> Handle(CreateAddTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: false), CancellationToken.None);
        if (token is not null)
        {
            throw new TokenAlreadyIndexedException(request.Token);
        }

        var latestBlock = await _mediator.Send(new GetBestBlockReceiptQuery(), CancellationToken.None);

        StandardTokenContractSummary summary;
        try
        {
            summary = await _mediator.Send(new CallCirrusGetStandardTokenContractSummaryQuery(request.Token,
                                                                                              latestBlock.Height,
                                                                                              includeBaseProperties: true,
                                                                                              includeTotalSupply: true), CancellationToken.None);
        }
        catch (Exception)
        {
            throw new InvalidDataException("token", "Unable to validate SRC token.");
        }

        token = new Token(request.Token, summary.Name, summary.Symbol, (int)summary.Decimals.Value, summary.Sats.Value,
                          summary.TotalSupply.Value, latestBlock.Height);

        var tokenId = await _mediator.Send(new MakeTokenCommand(token, latestBlock.Height));
        if (tokenId == 0) throw new Exception("Something went wrong indexing the token");

        var attributes = new List<TokenAttributeType> { TokenAttributeType.NonProvisional };

        WrappedTokenDetailsDto wrappedTokenDetails = null;
        var interfluxSummary = await _mediator.Send(new CallCirrusGetInterfluxTokenContractSummaryQuery(request.Token, latestBlock.Height), CancellationToken.None);
        if (interfluxSummary is not null)
        {
            attributes.Add(TokenAttributeType.Interflux);

            var tokenWrapped = new TokenWrapped(tokenId, interfluxSummary.Owner, interfluxSummary.NativeChain, interfluxSummary.NativeAddress, latestBlock.Height);
            var tokenWrappedId = await _mediator.Send(new MakeTokenWrappedCommand(tokenWrapped), CancellationToken.None);
            if (tokenWrappedId == 0) _logger.LogError("Something went wrong indexing the wrapped token mapping");

            wrappedTokenDetails = _mapper.Map<WrappedTokenDetailsDto>(tokenWrapped);
        }

        var attributesPersisted = await Task.WhenAll(attributes.Select(attribute =>
        {
            var tokenAttribute = new TokenAttribute(tokenId, attribute);
            return _mediator.Send(new MakeTokenAttributeCommand(tokenAttribute), CancellationToken.None);
        }));
        if (attributesPersisted.Any(a => !a)) _logger.LogError("Something went wrong indexing the token attributes");

        var tokenDto = _mapper.Map<TokenDto>(token);
        tokenDto.Attributes = attributes;
        tokenDto.WrappedToken = wrappedTokenDetails;
        return tokenDto;
    }
}
