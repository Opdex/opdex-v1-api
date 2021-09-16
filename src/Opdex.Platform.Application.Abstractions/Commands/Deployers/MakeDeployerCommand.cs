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
        public MakeDeployerCommand(Deployer deployer, ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Deployer = deployer ?? throw new ArgumentNullException(nameof(deployer), "Deployer must be provided.");
            BlockHeight = blockHeight;
        }

        public Deployer Deployer { get; }
        public ulong BlockHeight { get; }
    }
}
