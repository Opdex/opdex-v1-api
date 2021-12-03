using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

public class PersistTokenAttributeCommand : IRequest<bool>
{
    public PersistTokenAttributeCommand(TokenAttribute attribute)
    {
        Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute), "Attribute must be provided.");
    }

    public TokenAttribute Attribute { get; }
}