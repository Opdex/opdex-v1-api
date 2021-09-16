using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Deployers
{
    /// <summary>
    /// Creates or updates a market deployer and it's properties.
    /// </summary>
    public class CreateDeployerCommand : IRequest<long>
    {
        /// <summary>
        /// Create the create deployer command.
        /// </summary>
        /// <param name="deployer">The address of the deployer contract.</param>
        /// <param name="owner">The address of the owner of the deployer contract.</param>
        /// <param name="blockHeight">The block height the deployer was created or updated at.</param>
        /// <param name="isUpdate">Flag signaling if the command is to update or create a deployer.</param>
        public CreateDeployerCommand(Address deployer, Address owner, ulong blockHeight, bool isUpdate)
        {
            if (deployer == Address.Empty)
            {
                throw new ArgumentNullException(nameof(deployer), "Deployer address must be provided.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Deployer = deployer;
            Owner = owner;
            BlockHeight = blockHeight;
            IsUpdate = isUpdate;
        }

        public Address Deployer { get; }
        public Address Owner { get; }
        public ulong BlockHeight { get; }
        public bool IsUpdate { get; }
    }
}
