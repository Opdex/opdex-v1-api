using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Deployers
{
    public class MakeDeployerCommand : IRequest<long>
    {
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
