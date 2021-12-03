using System;
using MediatR;
using Opdex.Platform.Domain.Models.Deployers;

namespace Opdex.Platform.Application.Abstractions.Commands.Deployers;

/// <summary>
/// Create a make deployer command that rewinds, when necessary, and persist deployers.
/// </summary>
public class MakeDeployerCommand : IRequest<ulong>
{
    /// <summary>
    /// Creates the make deployer command.
    /// </summary>
    /// <param name="deployer">The Deployer domain model being made.</param>
    /// <param name="blockHeight">The block height of the update being made.</param>
    /// <param name="refreshPendingOwner">Flag to refresh the pending owner property of the deployer contract, default is false.</param>
    /// <param name="refreshOwner">Flag to refresh the owner property of the deployer contract, default is false.</param>
    public MakeDeployerCommand(Deployer deployer, ulong blockHeight, bool refreshPendingOwner = false, bool refreshOwner = false)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Deployer = deployer ?? throw new ArgumentNullException(nameof(deployer), "Deployer must be provided.");
        BlockHeight = blockHeight;
        RefreshPendingOwner = refreshPendingOwner;
        RefreshOwner = refreshOwner;
    }

    public Deployer Deployer { get; }
    public ulong BlockHeight { get; }
    public bool RefreshPendingOwner { get; }
    public bool RefreshOwner { get; }
    public bool Refresh => RefreshPendingOwner || RefreshOwner;
}