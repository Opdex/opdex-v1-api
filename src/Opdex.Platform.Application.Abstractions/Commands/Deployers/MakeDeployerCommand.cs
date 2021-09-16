using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Deployers
{
    /// <summary>
    /// Create a make deployer command that rewinds, when necessary, and persist deployers.
    /// </summary>
    public class MakeDeployerCommand : IRequest<long>
    {
        /// <summary>
        /// Creates the make deployer command.
        /// </summary>
        /// <param name="deployer">The Deployer domain model being made.</param>
        /// <param name="blockHeight">The block height of the update being made.</param>
        /// <param name="rewind">Flag to signal a rewind to the specified block height.</param>
        public MakeDeployerCommand(Deployer deployer, ulong blockHeight, bool rewind = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Deployer = deployer ?? throw new ArgumentNullException(nameof(deployer), "Deployer must be provided.");
            BlockHeight = blockHeight;
            Rewind = rewind;
        }

        public Deployer Deployer { get; }
        public ulong BlockHeight { get; }
        public bool Rewind { get; }
    }
}
