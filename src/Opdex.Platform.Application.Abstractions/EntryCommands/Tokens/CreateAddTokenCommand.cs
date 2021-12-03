using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;

/// <summary>
/// Command to add a new token to the database and return the token details.
/// </summary>
public class CreateAddTokenCommand : IRequest<TokenDto>
{
    /// <summary>
    /// Creates a command to add and validate a token and return the token details.
    /// </summary>
    /// <param name="token">The address of the token.</param>
    public CreateAddTokenCommand(Address token)
    {
        if (token == Address.Empty)
        {
            throw new ArgumentNullException(nameof(token), "Token address must be provided.");
        }

        Token = token;
    }

    public Address Token { get; }
}