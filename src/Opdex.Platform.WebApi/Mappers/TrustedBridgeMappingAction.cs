using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System;

namespace Opdex.Platform.WebApi.Mappers;

public class TrustedBridgeMappingAction : IMappingAction<WrappedTokenDetailsDto, WrappedTokenDetailsResponseModel>
{
    private readonly InterfluxConfiguration _interfluxConfiguration;

    public TrustedBridgeMappingAction(InterfluxConfiguration interfluxConfiguration)
    {
        _interfluxConfiguration = interfluxConfiguration ?? throw new ArgumentNullException(nameof(interfluxConfiguration));
    }

    public void Process(WrappedTokenDetailsDto source, WrappedTokenDetailsResponseModel destination, ResolutionContext context)
    {
        destination.Trusted = source.Custodian == _interfluxConfiguration.MultiSigContractAddress;
    }
}
