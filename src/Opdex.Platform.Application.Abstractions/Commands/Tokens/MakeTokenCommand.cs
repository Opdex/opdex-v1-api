using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens;

/// <summary>
/// Create a make token command to update and/or persist a token domain model.
/// </summary>
public class MakeTokenCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to create the make token command.
    /// </summary>
    /// <param name="token">The token domain model to update and/or persist.</param>
    /// <param name="blockHeight">The block height to refresh optional properties at.</param>
    /// <param name="refreshTotalSupply">Optional flag to signal a refresh of the token's total supply, default is false.</param>
    public MakeTokenCommand(Token token, ulong blockHeight, bool refreshTotalSupply = false)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Token = token ?? throw new ArgumentNullException(nameof(token), "Token must be provided.");
        BlockHeight = blockHeight;
        RefreshTotalSupply = refreshTotalSupply;
    }

    public Token Token { get; }
    public ulong BlockHeight { get; }
    public bool RefreshTotalSupply { get; }
    public bool Refresh => RefreshTotalSupply;
}
