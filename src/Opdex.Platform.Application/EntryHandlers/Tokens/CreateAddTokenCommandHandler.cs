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
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens;

public class CreateAddTokenCommandHandler : IRequestHandler<CreateAddTokenCommand, TokenDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly InterfluxConfiguration _interfluxConfiguration;
    private readonly ILogger<CreateAddTokenCommandHandler> _logger;

    public CreateAddTokenCommandHandler(IMapper mapper, IMediator mediator, InterfluxConfiguration interfluxConfiguration,
        ILogger<CreateAddTokenCommandHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _interfluxConfiguration = interfluxConfiguration ?? throw new ArgumentNullException(nameof(interfluxConfiguration));
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

        var interfluxSummary = await _mediator.Send(new CallCirrusGetInterfluxTokenContractSummaryQuery(request.Token, latestBlock.Height), CancellationToken.None);
        if (interfluxSummary is not null && !ValidateWrappedToken(interfluxSummary))
        {
            throw new InvalidDataException("token", "Wrapped token must be owned by Interflux multisig wallet.");
        };

        token = new Token(request.Token, summary.Name, summary.Symbol, (int)summary.Decimals.Value, summary.Sats.Value,
                          summary.TotalSupply.Value, latestBlock.Height);

        var tokenId = await _mediator.Send(new MakeTokenCommand(token, latestBlock.Height));
        if (tokenId == 0) _logger.LogError("Something went wrong indexing the token");

        var nonProvisionalAttribute = new TokenAttribute(tokenId, TokenAttributeType.NonProvisional);
        var nonProvisionalAttributePersisted = await _mediator.Send(new MakeTokenAttributeCommand(nonProvisionalAttribute));
        if (!nonProvisionalAttributePersisted) _logger.LogError("Something went wrong indexing the non provisional token");

        WrappedTokenDetailsDto wrappedTokenDetails = null;
        if (interfluxSummary is not null)
        {
            var interfluxAttribute = new TokenAttribute(tokenId, TokenAttributeType.Interflux);
            var interfluxAttributePersisted = await _mediator.Send(new MakeTokenAttributeCommand(interfluxAttribute));
            if (!interfluxAttributePersisted) _logger.LogError("Something went wrong indexing the interflux attribute");

            var tokenChain = new TokenChain(tokenId, interfluxSummary.NativeChain, interfluxSummary.NativeAddress);
            var tokenChainId = await _mediator.Send(new MakeTokenChainCommand(tokenChain), CancellationToken.None);
            if (tokenChainId == 0) _logger.LogError("Something went wrong indexing the wrapped token mapping");

            wrappedTokenDetails = _mapper.Map<WrappedTokenDetailsDto>(tokenChain);
        }

        var tokenDto = _mapper.Map<TokenDto>(token);
        tokenDto.NativeToken = wrappedTokenDetails;
        tokenDto.Attributes = new List<TokenAttributeType> {TokenAttributeType.NonProvisional, TokenAttributeType.Interflux};
        return tokenDto;
    }

    private bool ValidateWrappedToken(InterfluxTokenContractSummary summary)
    {
        return summary.Owner == _interfluxConfiguration.MultiSigContractAddress;
    }
}
