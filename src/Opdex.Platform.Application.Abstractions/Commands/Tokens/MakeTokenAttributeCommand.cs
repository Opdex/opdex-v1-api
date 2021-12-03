using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens;

/// <summary>
/// Assign and persist a new attribute to a token if it doesn't already exist.
/// </summary>
public class MakeTokenAttributeCommand : IRequest<bool>
{
    /// <summary>
    /// Constructor to build a make token attribute command.
    /// </summary>
    /// <param name="tokenAttribute">The token attribute to make.</param>
    public MakeTokenAttributeCommand(TokenAttribute tokenAttribute)
    {
        TokenAttribute = tokenAttribute ?? throw new ArgumentNullException(nameof(tokenAttribute), "Token attribute must be provided.");
    }

    public TokenAttribute TokenAttribute { get; }
}