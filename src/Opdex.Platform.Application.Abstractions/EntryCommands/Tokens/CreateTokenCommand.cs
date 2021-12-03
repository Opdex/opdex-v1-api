using MediatR;
using Opdex.Platform.Common.Models;
using System;

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
    /// <param name="blockHeight">The block height the token is being added at.</param>
    public CreateTokenCommand(Address token, ulong blockHeight)
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
    }

    public Address Token { get; }
    public ulong BlockHeight { get; }
}