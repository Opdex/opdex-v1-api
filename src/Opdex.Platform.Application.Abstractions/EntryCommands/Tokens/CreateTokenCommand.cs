using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;

/// <summary>
/// Create a new token and persist it to the database.
/// </summary>
public class CreateTokenCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor for the create token command.
    /// </summary>
    /// <param name="token">The SRC token contract address.</param>
    /// <param name="attributes">List of token attributes to apply to the token.</param>
    /// <param name="blockHeight">The block height the token is being added at.</param>
    public CreateTokenCommand(Address token, IEnumerable<TokenAttributeType> attributes, ulong blockHeight)
    {
        if (token == Address.Empty)
        {
            throw new ArgumentNullException(nameof(token), "Token address must be provided.");
        }

        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Token = token;
        BlockHeight = blockHeight;
        Attributes = attributes;
    }

    public Address Token { get; }
    public IEnumerable<TokenAttributeType> Attributes { get; }
    public ulong BlockHeight { get; }
}
