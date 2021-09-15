using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Deployers
{
    public class CreateDeployerCommand : IRequest<long>
    {
        public CreateDeployerCommand(Address deployer, Address owner, ulong blockHeight, bool isUpdate = false)
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
